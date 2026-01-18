using CuaHangXeMoHinh.Models;
using System.ComponentModel.DataAnnotations;

namespace CuaHangXeMoHinh.Areas.Admin.Models.ViewModels
{
    public class OrderIndexViewModel
    {
        public List<Order> Orders { get; set; }
        public string Search { get; set; }
        public OrderStatus? Status { get; set; }
        public string SortBy { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
        public int StartPage => Math.Max(1, CurrentPage - 2);
        public int EndPage => Math.Min(TotalPages, CurrentPage + 2);
    }

    public class OrderEditViewModel : OrderEditPostViewModel
    {
        public string UserFullName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus CurrentStatus { get; set; }
    }

    public class OrderEditPostViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn trạng thái")]
        [Display(Name = "Trạng thái")]
        public OrderStatus Status { get; set; }

        [Display(Name = "Phí vận chuyển")]
        [Range(0, 10000000, ErrorMessage = "Phí vận chuyển phải từ 0 đến 10,000,000")]
        public decimal ShippingFee { get; set; }

        [Display(Name = "Giảm giá")]
        [Range(0, 10000000, ErrorMessage = "Giảm giá phải từ 0 đến 10,000,000")]
        public decimal Discount { get; set; }

        [Display(Name = "Ghi chú")]
        [MaxLength(1000, ErrorMessage = "Ghi chú không được quá 1000 ký tự")]

        public string? Note { get; set; }
    }

}