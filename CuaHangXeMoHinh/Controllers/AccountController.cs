using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Models;
using CuaHangXeMoHinh.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.EntityFrameworkCore;

namespace CuaHangXeMoHinh.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            var model = await GetProfileViewModelAsync(user);


            ViewBag.ActiveTab = TempData["ActiveTab"]?.ToString() ?? "overview";

            if (TempData["Error_OldPassword"] != null)
            {
                ModelState.AddModelError("ChangePassword.OldPassword", TempData["Error_OldPassword"].ToString());
            }
            if (TempData["GeneralValidationError"] != null)
            {
                ModelState.AddModelError(string.Empty, TempData["GeneralValidationError"].ToString());
            }


            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([Bind(Prefix = "ChangePassword")] ChangePasswordViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");
            TempData["ActiveTab"] = "change-password";
            if (!ModelState.IsValid)
            {
                var profileModel = await GetProfileViewModelAsync(user);
                profileModel.ChangePassword = model;
                return View("Profile", profileModel);
            }

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Đã đổi mật khẩu thành công!";
                return RedirectToAction("Profile", "Account", null, "change-password");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    TempData["GeneralValidationError"] = error.Description;
                }

                return RedirectToAction("Profile");
            }


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress([Bind(Prefix = "NewAddress")] AddressViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            TempData["ActiveTab"] = "addresses";
            if (!ModelState.IsValid)
            {
                var profileModel = await GetProfileViewModelAsync(user);

                profileModel.NewAddress = model;

                return View("Profile", profileModel);
            }

            if (model.IsDefault)
            {
                var defaultAddresses = await _context.Addresses
                    .Where(a => a.UserId == user.Id && a.IsDefault)
                    .ToListAsync();

                foreach (var addr in defaultAddresses)
                {
                    addr.IsDefault = false;
                }
            }

            var address = new Address
            {
                UserId = user.Id,
                Line1 = model.Line1,
                Line2 = model.Line2,
                City = model.City,
                Province = model.Province,
                IsDefault = model.IsDefault
            };

            try
            {
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm địa chỉ mới thành công!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi thêm địa chỉ.";
            }

            return RedirectToAction("Profile", "Account", null, "addresses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            TempData["ActiveTab"] = "addresses";

            var address = await _context.Addresses
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == user.Id);

            if (address != null)
            {
                var isAddressInUse = await _context.Orders
                    .AnyAsync(o => o.ShippingAddressId == id);

                if (isAddressInUse)
                {
                    TempData["ErrorMessage"] = "Không thể xóa địa chỉ này vì nó đang được sử dụng trong các đơn hàng đã tạo.";
                }
                else
                {
                    var addressCount = await _context.Addresses
                        .CountAsync(a => a.UserId == user.Id);

                    if (addressCount > 1)
                    {
                        _context.Addresses.Remove(address);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Đã xóa địa chỉ thành công!";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Không thể xóa địa chỉ duy nhất của bạn.";
                    }
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Địa chỉ không tồn tại hoặc đã bị xóa.";
            }

            return RedirectToAction("Profile", "Account", null, "addresses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            TempData["ActiveTab"] = "addresses";

            var addresses = await _context.Addresses
                .Where(a => a.UserId == user.Id)
                .ToListAsync();

            var selectedAddress = addresses.FirstOrDefault(a => a.Id == id);

            if (selectedAddress != null)
            {
                foreach (var addr in addresses)
                {
                    addr.IsDefault = false;
                }

                selectedAddress.IsDefault = true;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã đặt địa chỉ mặc định thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy địa chỉ yêu cầu.";
            }

            return RedirectToAction("Profile", "Account", null, "addresses");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(string fullName, string phoneNumber, DateTime? dateOfBirth)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login");

            TempData["ActiveTab"] = "overview";

            user.FullName = fullName;
            user.PhoneNumber = phoneNumber;
            user.DateOfBirth = dateOfBirth;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                return RedirectToAction("Profile");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    TempData["GeneralValidationError"] = error.Description;
                }

                return RedirectToAction("Profile");
            }
        }

        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, String returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    {
                        user.LastLogin = DateTime.Now;
                        await _userManager.UpdateAsync(user);
                    }
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    return RedirectToAction("Index", "Home");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Tài khoản của bạn đã bị khóa. Vui lòng liên hệ Quản trị viên.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không đúng");
                }
            }
            return View(model);
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    DateOfBirth = model.DateOfBirth,
                    CreatedAt = DateTime.Now,
                    LastLogin = DateTime.Now,
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Customer");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    if (error.Code == "DuplicateUserName")
                    {
                        ModelState.AddModelError(string.Empty, $"Email {model.Email} đã được sử dụng. Vui lòng chọn email khác.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        private async Task<AccountViewModel> GetProfileViewModelAsync(User user)
        {
            var addresses = await _context.Addresses
                .Where(a => a.UserId == user.Id)
                .Select(a => new AddressViewModel
                {
                    Id = a.Id,
                    Line1 = a.Line1,
                    Line2 = a.Line2,
                    City = a.City,
                    Province = a.Province,
                    IsDefault = a.IsDefault
                })
                .ToListAsync();

            var orders = await _context.Orders
                .Where(o => o.UserId == user.Id)
                .Include(o => o.Items).ThenInclude(i => i.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(10)
                .ToListAsync();

            return new AccountViewModel
            {
                User = user,
                Addresses = addresses,
                Orders = orders,
                ChangePassword = new ChangePasswordViewModel(),
                NewAddress = new AddressViewModel()
            };
        }
        [HttpGet]
        public IActionResult AccessDenied()
        {
            TempData["AccessDeniedMessage"] = "Bạn cần quyền quản trị để truy cập trang này.";

            return View();
        }
    }
}