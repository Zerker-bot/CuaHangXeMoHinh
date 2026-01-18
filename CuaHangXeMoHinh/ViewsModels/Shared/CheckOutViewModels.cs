// Tạo file CheckoutViewModel.cs
using CuaHangXeMoHinh.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CuaHangXeMoHinh.ViewModels
{
    public class CheckoutViewModel
    {
        public User User { get; set; }
        public List<Address> Addresses { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public int? SelectedAddressId { get; set; }

        public List<CartItemViewModel> CartItems { get; set; }

        // Chỉ cần Subtotal, các tính toán còn lại Order sẽ tự xử lý
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; } = 30000;
        public decimal Discount { get; set; } = 0;
        public string? Note { get; set; } = string.Empty;
        // Chỉ tính Total để hiển thị preview, Order sẽ tính lại khi lưu
        public decimal Total => Subtotal + ShippingFee - Discount;

    }

    public class CartItemViewModel
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }

        public decimal TotalPrice => Product?.Price * Quantity ?? 0;
    }
}