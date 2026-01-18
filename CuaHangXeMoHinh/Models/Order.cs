
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CuaHangXeMoHinh.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public User? User { get; set; }
        public int? ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Precision(18, 2)]
        public decimal ShippingFee { get; set; }
        [Precision(18, 2)]
        public decimal Discount { get; set; }
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        [NotMapped]
        public decimal CalculatedTotal
        {
            get
            {
                decimal itemsTotal = 0;
                foreach (var i in Items) itemsTotal += i.UnitPrice * i.Quantity;
                return itemsTotal + ShippingFee - Discount;
            }
        }
        [MaxLength(1000)]
        [Display(Name = "Ghi chú")]
        public string? Note { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Navigation property cho logs
        public ICollection<OrderStatusLog> StatusLogs { get; set; } = new List<OrderStatusLog>();
    }
    public enum OrderStatus
    {

        [Display(Name = "Chờ xử lý")]
        Pending = 0,

        [Display(Name = "Đang xử lý")]
        Processing = 1,

        [Display(Name = "Đang vận chuyển")]
        Shipped = 2,

        [Display(Name = "Giao hàng thành công")]
        Delivered = 3,

        // --- QUY TRÌNH TRẢ HÀNG ---
        [Display(Name = "Yêu cầu trả hàng")]
        ReturnRequested = 4, // Khách ấn nút trả hàng, chờ Shop duyệt

        // --- CÁC TRẠNG THÁI KẾT THÚC (FINAL) ---
        [Display(Name = "Đã trả hàng")]
        Returned = 5,  // Shop đã nhận lại hàng và hoàn tiền

        [Display(Name = "Từ chối trả hàng")]
        ReturnDenied = 6, // Shop không đồng ý nhận lại (do quá hạn, lỗi người dùng...)

        [Display(Name = "Đã huỷ")]
        Cancelled = -1
    }

}
