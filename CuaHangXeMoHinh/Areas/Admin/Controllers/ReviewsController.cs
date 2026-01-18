using CuaHangXeMoHinh.Controllers;
using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangXeMoHinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]

    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;

        }

        // GET: Admin/Reviews
        public async Task<IActionResult> Index([FromQuery] string status)
        {


            if (String.IsNullOrEmpty(status))
            {
                status = "pending";
            }


            var query = _context.Reviews.AsQueryable();

            ViewBag.CurrentStatus = status;
            ViewBag.PendingCount = await _context.Reviews.CountAsync(r => r.IsApproved == false && r.Response == null);
            ViewBag.ApprovedCount = await _context.Reviews.CountAsync(r => r.IsApproved == true);
            ViewBag.RejectedCount = await _context.Reviews.CountAsync(r => r.IsApproved == false && r.Response != null);
            switch (status)
            {
                case "approved":
                    query = query.Where(r => r.IsApproved == true);
                    break;
                case "rejected":
                    query = query.Where(r => r.IsApproved == false && r.Response != null);
                    break;
                case "pending":
                default:
                    query = query.Where(r => r.IsApproved == false && r.Response == null);
                    break;
            }

            var reviews = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
            return View(reviews);
        }

        // GET: Admin/Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.User)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (review == null)
            {
                return NotFound();
            }

            ViewData["ActiveMenu"] = "reviews";
            return View(review);
        }

        // POST: Admin/Reviews/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string response)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            review.IsApproved = true;
            review.Response = string.IsNullOrWhiteSpace(response) ? "Đã được duyệt bởi quản trị viên." : response;
            _context.Update(review);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Đã duyệt đánh giá #{review.Id} thành công!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Reviews/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string response)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            review.IsApproved = false;
            review.Response = string.IsNullOrWhiteSpace(response) ? "Đánh giá không đạt yêu cầu." : response;
            _context.Update(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã từ chối đánh giá #{review.Id}!";
            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/Reviews/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa đánh giá #{review.Id} thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}