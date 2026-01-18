using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Models;
using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangXeMoHinh.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public OrderController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> History(string? status, string? search, int page = 1, int pageSize = 10)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                    .ThenInclude(p => p.Images)
                .Where(o => o.UserId == user.Id);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, out var statusEnum))
            {
                query = query.Where(o => o.Status == statusEnum);
                ViewBag.CurrentStatus = statusEnum;
            }

            if (!string.IsNullOrEmpty(search) && int.TryParse(search.Replace("#", ""), out var orderId))
            {
                query = query.Where(o => o.Id == orderId);
            }

            ViewBag.TotalOrders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .CountAsync();

            ViewBag.StatusCounts = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status.ToString(), x => x.Count);

            query = query.OrderByDescending(o => o.CreatedAt);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return View(orders);
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                .Include(o => o.ShippingAddress)
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == user.Id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> CancelOrder(int orderId, string? reason)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Người dùng không tồn tại" });
                }

                var order = await _context.Orders
                    .Include(o => o.Items)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == user.Id);

                if (order == null)
                {
                    return Json(new { success = false, message = "Đơn hàng không tồn tại" });
                }

                if (order.Status != OrderStatus.Pending)
                {
                    return Json(new { success = false, message = "Chỉ có thể huỷ đơn hàng ở trạng thái Chờ xử lý" });
                }

                order.Status = OrderStatus.Cancelled;

                foreach (var item in order.Items)
                {
                    var product = await _context.Products.FindAsync(item.ProductId);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                    }
                }


                await _context.SaveChangesAsync();


                return Json(new { success = true, message = "Đã huỷ đơn hàng thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Lỗi: {ex.Message}" });
            }
        }


    }
}
