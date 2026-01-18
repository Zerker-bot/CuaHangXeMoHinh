using System;
using System.ComponentModel.DataAnnotations;

namespace CuaHangXeMoHinh.Models
{
    public class OrderStatusLog
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public OrderStatus OldStatus { get; set; }

        [Required]
        public OrderStatus NewStatus { get; set; }

        [Required, MaxLength(100)]
        public string ChangedBy { get; set; }

        public DateTime ChangedAt { get; set; }

        [MaxLength(1000)]
        public string Note { get; set; }

        // Navigation property
        public Order Order { get; set; }
    }
}