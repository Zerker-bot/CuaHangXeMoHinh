using CuaHangXeMoHinh.Areas.Admin.Models.ViewModels;
using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Extensions;
using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangXeMoHinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Order
        public async Task<IActionResult> Index(
            string search = "",
            OrderStatus? status = null,
            string sortBy = "newest",
            int page = 1,
            int pageSize = 20)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.Id.ToString().Contains(search) ||
                    o.User.FullName.Contains(search) ||
                    o.User.Email.Contains(search) ||
                    o.User.PhoneNumber.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            query = sortBy switch
            {
                "oldest" => query.OrderBy(o => o.CreatedAt),
                "total_asc" => query.OrderBy(o => o.TotalAmount),
                "total_desc" => query.OrderByDescending(o => o.TotalAmount),
                _ => query.OrderByDescending(o => o.CreatedAt)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var viewModel = new OrderIndexViewModel
            {
                Orders = orders,
                Search = search,
                Status = status,
                SortBy = sortBy,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                TotalPages = totalPages
            };

            return View(viewModel);
        }

        // GET: Admin/Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.ShippingAddress)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Brand)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.TotalAmount == 0 && order.Items.Any())
            {
                order.TotalAmount = order.CalculatedTotal;
            }

            return View(order);
        }

        // GET: Admin/Order/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var order = await _context.Orders
       .Include(o => o.User)
       .Include(o => o.ShippingAddress)
       .Include(o => o.Items)
           .ThenInclude(i => i.Product)
       .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            var model = new OrderEditViewModel
            {
                Id = order.Id,
                Status = order.Status,
                ShippingFee = order.ShippingFee,
                Discount = order.Discount,
                Note = order.Note,
                CurrentStatus = order.Status,
                UserFullName = order.User?.FullName,
                TotalAmount = order.TotalAmount,
                CreatedAt = order.CreatedAt
            };

            ViewBag.Order = order;
            return View(model);
        }

        // POST: Admin/Order/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderEditPostViewModel model)
        {
            var order = await _context.Orders
    .Include(o => o.User)
    .Include(o => o.ShippingAddress)
    .Include(o => o.Items)
        .ThenInclude(i => i.Product)
    .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                var editViewModel = new OrderEditViewModel
                {
                    Id = model.Id,
                    Status = model.Status,
                    ShippingFee = model.ShippingFee,
                    Discount = model.Discount,
                    Note = model.Note,
                    CurrentStatus = order.Status,
                    UserFullName = order.User?.FullName,
                    TotalAmount = order.TotalAmount,
                    CreatedAt = order.CreatedAt
                };

                ViewBag.Order = order;
                return View(editViewModel);
            }

            var oldStatus = order.Status;

            order.Status = model.Status;
            order.ShippingFee = model.ShippingFee;
            order.Discount = model.Discount;
            order.Note = model.Note;
            order.UpdatedAt = DateTime.Now;

            order.TotalAmount = order.CalculatedTotal;

            try
            {
                await _context.SaveChangesAsync();

                if (oldStatus != model.Status)
                {
                    var statusLog = new OrderStatusLog
                    {
                        OrderId = order.Id,
                        OldStatus = oldStatus,
                        NewStatus = model.Status,
                        ChangedBy = User.Identity.Name,
                        ChangedAt = DateTime.Now,
                        Note = $"Trạng thái thay đổi từ {oldStatus.GetDisplayName()} sang {model.Status.GetDisplayName()}"
                    };
                    _context.OrderStatusLogs.Add(statusLog);
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Cập nhật đơn hàng thành công!";
                return RedirectToAction(nameof(Details), new { id = order.Id });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

        }

        // POST: Admin/Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Cancel(int id, string reason)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            if (order.Status == OrderStatus.Delivered || order.Status == OrderStatus.Cancelled)
            {
                TempData["ErrorMessage"] = "Không thể hủy đơn hàng ở trạng thái hiện tại!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var oldStatus = order.Status;
            order.Status = OrderStatus.Cancelled;
            order.Note = string.IsNullOrEmpty(reason) ?
                "Đơn hàng đã bị hủy bởi quản trị viên." :
                $"Đơn hàng đã bị hủy: {reason}";
            order.UpdatedAt = DateTime.Now;

            var statusLog = new OrderStatusLog
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = OrderStatus.Cancelled,
                ChangedBy = User.Identity.Name,
                ChangedAt = DateTime.Now,
                Note = $"Hủy đơn hàng: {reason}"
            };

            _context.OrderStatusLogs.Add(statusLog);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã hủy đơn hàng thành công!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Admin/Order/ProcessReturn/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ProcessReturn(int id, bool approve, string response)
        {
            var order = await _context.Orders
                .Include(o => o.User)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null || order.Status != OrderStatus.ReturnRequested)
            {
                TempData["ErrorMessage"] = "Không thể xử lý yêu cầu trả hàng!";
                return RedirectToAction(nameof(Details), new { id });
            }

            var oldStatus = order.Status;
            order.Status = approve ? OrderStatus.Returned : OrderStatus.ReturnDenied;
            order.Note = response;
            order.UpdatedAt = DateTime.Now;

            var statusLog = new OrderStatusLog
            {
                OrderId = order.Id,
                OldStatus = oldStatus,
                NewStatus = order.Status,
                ChangedBy = User.Identity.Name,
                ChangedAt = DateTime.Now,
                Note = approve ? "Chấp nhận trả hàng" : "Từ chối trả hàng"
            };

            _context.OrderStatusLogs.Add(statusLog);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = approve ?
                "Đã chấp nhận yêu cầu trả hàng!" :
                "Đã từ chối yêu cầu trả hàng!";
            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Admin/Order/StatusLogs/5
        public async Task<IActionResult> StatusLogs(int id)
        {
            var logs = await _context.OrderStatusLogs
                .Where(l => l.OrderId == id)
                .OrderByDescending(l => l.ChangedAt)
                .ToListAsync();
            ViewBag.OrderId = id;
            return View(logs);
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}