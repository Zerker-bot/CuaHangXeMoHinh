using CuaHangXeMoHinh.Areas.Admin.ViewModels;
using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Extensions;
using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangXeMoHinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]

    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var startOfMonth = new DateTime(today.Year, today.Month, 1);
            var startOfYear = new DateTime(today.Year, 1, 1);

            var dashboardData = new DashboardViewModel
            {
                TotalProducts = await _context.Products.CountAsync(),
                TotalOrders = await _context.Orders.CountAsync(),
                TotalUsers = await _context.Users.CountAsync(),
                PendingReviews = await _context.Reviews.CountAsync(r => !r.IsApproved),
                PendingOrders = await _context.Orders.CountAsync(o => o.Status == OrderStatus.Pending),
                ReturnRequests = await _context.Orders.CountAsync(o => o.Status == OrderStatus.ReturnRequested),

                TodayRevenue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt.Date == today)
                    .SumAsync(o => o.TotalAmount),
                MonthRevenue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt >= startOfMonth)
                    .SumAsync(o => o.TotalAmount),
                YearRevenue = await _context.Orders
                    .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt >= startOfYear)
                    .SumAsync(o => o.TotalAmount),

                OrderStatusDistribution = await GetOrderStatusDistribution(),
                RecentSalesData = await GetRecentSalesData(7),
                TopSellingProducts = await GetTopSellingProducts(10),
                UserRegistrations = await GetUserRegistrations(30),

                RecentOrders = await GetRecentOrders(10),
                RecentPendingReviews = await GetRecentPendingReviews(5),
                LowStockProducts = await GetLowStockProducts(10),
                RecentReturnRequests = await GetRecentReturnRequests(5),

                NegativeReviews = await GetNegativeReviews(),
                UnansweredReturnRequests = await GetUnansweredReturnRequests(),

                RecentActivities = await GetRecentActivities(20),

                SystemInfo = new SystemInfoViewModel
                {
                    TotalCategories = await _context.Categories.CountAsync(),
                    TotalBrands = await _context.Brands.CountAsync(),
                    TotalShippingAddresses = await _context.Addresses.CountAsync(),
                    AverageRating = await _context.Reviews
                        .Where(r => r.IsApproved)
                        .AverageAsync(r => (double?)r.Rating) ?? 0
                }
            };

            return View(dashboardData);
        }


        private async Task<Dictionary<string, int>> GetOrderStatusDistribution()
        {
            var rawData = await _context.Orders
                .GroupBy(o => o.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var result = rawData.ToDictionary(
                x => x.Status.GetDisplayName(),
                x => x.Count
            );

            return result;
        }

        private async Task<List<SalesDataPoint>> GetRecentSalesData(int days)
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-days + 1);

            var salesData = await _context.Orders
                .Where(o => o.Status == OrderStatus.Delivered &&
                           o.CreatedAt.Date >= startDate &&
                           o.CreatedAt.Date <= endDate)
                .GroupBy(o => o.CreatedAt.Date)
                .Select(g => new SalesDataPoint
                {
                    Date = g.Key,
                    Amount = g.Sum(o => o.TotalAmount),
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var result = new List<SalesDataPoint>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var existing = salesData.FirstOrDefault(x => x.Date == date);
                result.Add(existing ?? new SalesDataPoint { Date = date, Amount = 0, Count = 0 });
            }

            return result;
        }

        private async Task<List<TopProductViewModel>> GetTopSellingProducts(int count)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .GroupBy(oi => oi.ProductId)
                .Select(g => new TopProductViewModel
                {
                    ProductId = g.Key,
                    ProductName = g.First().Product.Name,
                    TotalSold = g.Sum(oi => oi.Quantity),
                    Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(count)
                .ToListAsync();
        }

        private async Task<List<UserRegistrationData>> GetUserRegistrations(int days)
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-days + 1);

            var registrations = await _context.Users
                .Where(u => u.CreatedAt.Date >= startDate && u.CreatedAt.Date <= endDate)
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new UserRegistrationData
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();

            var result = new List<UserRegistrationData>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var existing = registrations.FirstOrDefault(x => x.Date == date);
                result.Add(existing ?? new UserRegistrationData { Date = date, Count = 0 });
            }

            return result;
        }

        private async Task<List<Order>> GetRecentOrders(int count)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        private async Task<List<Review>> GetRecentPendingReviews(int count)
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => !r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        private async Task<List<Product>> GetLowStockProducts(int threshold = 10)
        {
            return await _context.Products
                .Where(p => p.Stock <= threshold)
                .OrderBy(p => p.Stock)
                .Take(10)
                .ToListAsync();
        }

        private async Task<List<Order>> GetRecentReturnRequests(int count)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == OrderStatus.ReturnRequested)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }


        private async Task<List<Review>> GetNegativeReviews()
        {
            return await _context.Reviews
                .Include(r => r.Product)
                .Include(r => r.User)
                .Where(r => r.Rating <= 2 && r.IsApproved)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToListAsync();
        }

        private async Task<List<Order>> GetUnansweredReturnRequests()
        {
            var cutoffTime = DateTime.UtcNow.AddHours(-48);
            return await _context.Orders
                .Include(o => o.User)
                .Where(o => o.Status == OrderStatus.ReturnRequested &&
                           o.CreatedAt < cutoffTime)
                .ToListAsync();
        }

        private async Task<List<ActivityLogViewModel>> GetRecentActivities(int count)
        {
            var rawOrderLogs = await _context.OrderStatusLogs
           .Include(l => l.Order)
           .OrderByDescending(l => l.ChangedAt)
           .Take(count / 2)
           .Select(l => new
           {
               l.Id,
               l.OrderId,
               l.OldStatus,
               l.NewStatus,
               l.ChangedBy,
               l.ChangedAt
           })
           .ToListAsync();

            var orderLogs = rawOrderLogs.Select(l => new ActivityLogViewModel
            {
                Id = l.Id,
                Type = "Order",
                Action = $"Đơn hàng #{l.OrderId} thay đổi từ {l.OldStatus.GetDisplayName()} sang {l.NewStatus.GetDisplayName()}",
                PerformedBy = l.ChangedBy,
                Timestamp = l.ChangedAt
            }).ToList();



            var productUpdates = await _context.Products
                .Where(p => p.UpdatedAt.HasValue)
                .OrderByDescending(p => p.UpdatedAt)
                .Take(count / 2)
                .Select(p => new ActivityLogViewModel
                {
                    Id = p.Id,
                    Type = "Product",
                    Action = $"Sản phẩm '{p.Name}' được cập nhật",
                    PerformedBy = "Hệ thống",
                    Timestamp = p.UpdatedAt.Value
                })
                .ToListAsync();

            var allActivities = orderLogs.Concat(productUpdates)
                .OrderByDescending(a => a.Timestamp)
                .Take(count)
                .ToList();

            return allActivities;
        }

        [HttpGet]
        public async Task<IActionResult> GetSalesChartData([FromQuery] int days = 30)
        {
            var data = await GetRecentSalesData(days);
            return Json(data);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderStatusData()
        {
            var data = await GetOrderStatusDistribution();
            return Json(data);
        }
    }
}