using CuaHangXeMoHinh.Models;
using System;
using System.Collections.Generic;

namespace CuaHangXeMoHinh.Areas.Admin.ViewModels
{
    public class DashboardViewModel
    {
        //  Tổng quan số liệu
        public int TotalProducts { get; set; }
        public int TotalOrders { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthRevenue { get; set; }
        public decimal YearRevenue { get; set; }
        public int TotalUsers { get; set; }
        public int PendingReviews { get; set; }
        public int PendingOrders { get; set; }
        public int ReturnRequests { get; set; }

        //Dữ liệu biểu đồ
        public Dictionary<string, int> OrderStatusDistribution { get; set; }
        public List<SalesDataPoint> RecentSalesData { get; set; }
        public List<TopProductViewModel> TopSellingProducts { get; set; }
        public List<UserRegistrationData> UserRegistrations { get; set; }

        // Danh sách nhanh
        public List<Order> RecentOrders { get; set; }
        public List<Review> RecentPendingReviews { get; set; }
        public List<Product> LowStockProducts { get; set; }
        public List<Order> RecentReturnRequests { get; set; }

        // Thông báo & cảnh báo
        public List<Order> OverdueOrders { get; set; }
        public List<Product> OutOfStockProducts { get; set; }
        public List<Review> NegativeReviews { get; set; }
        public List<Order> UnansweredReturnRequests { get; set; }

        // Hoạt động gần đây
        public List<ActivityLogViewModel> RecentActivities { get; set; }

        // Thông tin hệ thống
        public SystemInfoViewModel SystemInfo { get; set; }
    }

    public class SalesDataPoint
    {
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
    }

    public class TopProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int TotalSold { get; set; }
        public decimal Revenue { get; set; }
    }

    public class UserRegistrationData
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class ActivityLogViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Action { get; set; }
        public string PerformedBy { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class SystemInfoViewModel
    {
        public int TotalCategories { get; set; }
        public int TotalBrands { get; set; }
        public int TotalShippingAddresses { get; set; }
        public double AverageRating { get; set; }
    }
}