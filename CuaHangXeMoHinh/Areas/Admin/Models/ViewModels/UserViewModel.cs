namespace CuaHangXeMoHinh.Areas.Admin.Models.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public bool IsLocked { get; set; }
        public bool EmailConfirmed { get; set; }
    }

    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public List<RoleSelectionViewModel> UserRoles { get; set; } = new List<RoleSelectionViewModel>();
    }

    public class RoleSelectionViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}