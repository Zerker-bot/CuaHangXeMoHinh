using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CuaHangXeMoHinh.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()?
                            .Name ?? enumValue.ToString();
        }

        // 2. Hàm lấy màu Badge (Bootstrap class)
        public static string GetBadgeClass(this Enum enumValue)
        {
            return enumValue.ToString() switch
            {
                // Nhóm chờ/đang xử lý
                "Pending" => "bg-secondary",
                "Processing" => "bg-info",
                "Shipped" => "bg-primary",

                // Nhóm thành công
                "Delivered" => "bg-success",

                // Nhóm Trả hàng (Mới)
                "ReturnRequested" => "bg-warning text-dark",
                "Returned" => "bg-dark",
                "ReturnDenied" => "bg-danger",

                // Nhóm Huỷ
                "Cancelled" => "bg-danger",

                // Mặc định
                _ => "bg-secondary"
            };
        }
        public static string GetColorHex(this Enum enumValue)
        {
            return enumValue.ToString() switch
            {
                // Nhóm chờ/đang xử lý
                "Pending" => "#ffc107",
                "Processing" => "#17a2b8",
                "Shipped" => "#007bff",

                // Nhóm thành công
                "Delivered" => "#28a745",

                // Nhóm Trả hàng
                "ReturnRequested" => "#fd7e14",
                "Returned" => "#343a40",
                "ReturnDenied" => "#dc3545",

                // Nhóm Huỷ
                "Cancelled" => "#dc3545",

                // Mặc định (Xám nhạt)
                _ => "#6c757d"
            };
        }
    }
}
