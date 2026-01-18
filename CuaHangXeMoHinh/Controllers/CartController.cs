using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Models;
using CuaHangXeMoHinh.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace CuaHangXeMoHinh.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartController> _logger;
        private const string CART_SESSION_KEY = "ShoppingCart";

        public CartController(ApplicationDbContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var cart = GetCartFromSession();
                var cartItems = await GetCartItemsAsync(cart);

                decimal total = CalculateCartTotal(cartItems);

                ViewBag.CartTotal = total;
                ViewBag.CartItemCount = cart.Count;

                return View(cartItems);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải giỏ hàng");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải giỏ hàng. Vui lòng thử lại.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1)
        {
            _logger.LogWarning($"Add() CALLED — product={productId}, quantity={quantity}, time={DateTime.Now}");

            try
            {
                var product = await _context.Products
                    .Include(p => p.Images)
                    .FirstOrDefaultAsync(p => p.Id == productId);

                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                if (product.Stock < quantity)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Sản phẩm '{product.Name}' chỉ còn {product.Stock} sản phẩm trong kho."
                    });
                }

                var cart = GetCartFromSession();

                var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity = existingItem.Quantity + quantity;

                    if (product.Stock < existingItem.Quantity)
                    {
                        return Json(new
                        {
                            success = false,
                            message = $"Số lượng tối đa cho '{product.Name}' là {product.Stock}."
                        });
                    }
                }
                else
                {
                    cart.Add(new CartSessionItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        AddedDate = DateTime.Now
                    });
                }

                SaveCartToSession(cart);

                var totalTypes = cart.Count;

                return Json(new
                {
                    success = true,
                    message = $"Đã thêm '{product.Name}' vào giỏ hàng.",
                    cartItemCount = totalTypes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm sản phẩm vào giỏ hàng");
                return Json(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau."
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int productId, int quantity)
        {

            try
            {
                if (quantity < 1)
                {
                    return Json(new { success = false, message = "Số lượng phải lớn hơn 0." });
                }

                var product = await _context.Products.FindAsync(productId);
                if (product == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại." });
                }

                if (product.Stock < quantity)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Sản phẩm '{product.Name}' chỉ còn {product.Stock} sản phẩm trong kho."
                    });
                }

                var cart = GetCartFromSession();
                var existingItem = cart.FirstOrDefault(item => item.ProductId == productId);

                if (existingItem == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
                }

                existingItem.Quantity = quantity;
                existingItem.AddedDate = DateTime.Now;

                SaveCartToSession(cart);

                var cartItems = await GetCartItemsAsync(cart);

                var itemTotal = (product.Price * quantity).ToString("N0");
                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var shippingFee = 30000m;
                var total = subtotal + shippingFee;

                var totalTypes = cart.Count;

                return Json(new
                {
                    success = true,
                    message = "Đã cập nhật số lượng sản phẩm.",
                    itemTotal = itemTotal + " đ",
                    subtotal = subtotal.ToString("N0") + " đ",
                    total = total.ToString("N0") + " đ",
                    cartItemCount = totalTypes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật giỏ hàng");
                return Json(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau."
                });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int productId)
        {
            try
            {
                var cart = GetCartFromSession();
                var cartItem = cart.FirstOrDefault(item => item.ProductId == productId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Sản phẩm không tồn tại trong giỏ hàng." });
                }

                var product = await _context.Products.FindAsync(productId);
                var productName = product?.Name ?? "sản phẩm";

                cart.Remove(cartItem);
                SaveCartToSession(cart);

                var cartItems = await GetCartItemsAsync(cart);

                var subtotal = cartItems.Sum(item => item.TotalPrice);
                var shippingFee = 30000m;
                var total = subtotal + shippingFee;
                var totalTypes = cart.Count;

                return Json(new
                {
                    success = true,
                    message = $"Đã xóa '{productName}' khỏi giỏ hàng.",
                    subtotal = subtotal.ToString("N0") + " đ",
                    total = total.ToString("N0") + " đ",
                    cartItemCount = totalTypes
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa sản phẩm khỏi giỏ hàng");
                return Json(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau."
                });
            }
        }

        [Authorize] // Yêu cầu đăng nhập để thanh toán
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var cart = GetCartFromSession();

                if (!cart.Any())
                {
                    TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index");
                }

                var cartItems = await GetCartItemsAsync(cart);

                var outOfStockItems = new List<string>();
                foreach (var item in cartItems)
                {
                    if (item.Product.Stock < item.Quantity)
                    {
                        outOfStockItems.Add($"'{item.Product.Name}' (còn {item.Product.Stock})");
                    }
                }

                if (outOfStockItems.Any())
                {
                    TempData["ErrorMessage"] = $"Các sản phẩm sau không đủ số lượng: {string.Join(", ", outOfStockItems)}";
                    return RedirectToAction("Index");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _context.Users
                    .Include(u => u.Addresses)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                var addresses = user?.Addresses?.ToList() ?? new List<Address>();

                int? selectedAddressId = null;
                var defaultAddress = addresses.FirstOrDefault(a => a.IsDefault);
                if (defaultAddress != null)
                {
                    selectedAddressId = defaultAddress.Id;
                }

                var viewModel = new CheckoutViewModel
                {
                    User = user,
                    Addresses = addresses,
                    SelectedAddressId = selectedAddressId,
                    CartItems = cartItems,
                    ShippingFee = 30000,
                    Discount = 0
                };

                viewModel.Subtotal = CalculateCartTotal(cartItems);

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang thanh toán");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải trang thanh toán. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutViewModel model)
        {
            try
            {
                var cart = GetCartFromSession();

                if (!cart.Any())
                {
                    TempData["Error"] = "Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Checkout");
                }

                var cartItems = await GetCartItemsAsync(cart);

                var outOfStockItems = new List<string>();
                foreach (var item in cartItems)
                {
                    if (item.Product.Stock < item.Quantity)
                        outOfStockItems.Add(item.Product.Name);
                }

                if (outOfStockItems.Any())
                {
                    TempData["Error"] = "Các sản phẩm sau không đủ tồn kho: "
                                        + string.Join(", ", outOfStockItems);
                    return RedirectToAction("Checkout");
                }

                if (!model.SelectedAddressId.HasValue || model.SelectedAddressId.Value == 0)
                {
                    TempData["Error"] = "Vui lòng chọn địa chỉ giao hàng.";
                    return RedirectToAction("Checkout");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var address = await _context.Addresses
                    .FirstOrDefaultAsync(a => a.Id == model.SelectedAddressId && a.UserId == userId);

                if (address == null)
                {
                    TempData["Error"] = "Địa chỉ giao hàng không hợp lệ.";
                    return RedirectToAction("Checkout");
                }

                var order = new Order
                {
                    UserId = userId,
                    ShippingAddressId = model.SelectedAddressId,
                    ShippingFee = model.ShippingFee,
                    Discount = model.Discount,
                    Status = OrderStatus.Pending,
                    CreatedAt = DateTime.Now,
                    Note = model.Note
                };

                foreach (var cartItem in cartItems)
                {
                    order.Items.Add(new OrderItem
                    {
                        ProductId = cartItem.Product.Id,
                        Quantity = cartItem.Quantity,
                        UnitPrice = cartItem.Product.Price
                    });
                }

                order.TotalAmount = order.CalculatedTotal;

                using var transaction = await _context.Database.BeginTransactionAsync();

                try
                {
                    foreach (var cartItem in cartItems)
                    {
                        var product = await _context.Products.FindAsync(cartItem.Product.Id);
                        product.Stock -= cartItem.Quantity;
                        product.UpdatedAt = DateTime.Now;
                        _context.Products.Update(product);
                    }

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    ClearCart();

                    return RedirectToAction("OrderSuccess", new { orderId = order.Id });
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đặt hàng");
                TempData["Error"] = "Đã xảy ra lỗi, vui lòng thử lại.";
                return RedirectToAction("Checkout");
            }
        }


        [Authorize]
        public async Task<IActionResult> OrderSuccess(int orderId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var order = await _context.Orders
                    .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Images)
                    .Include(o => o.ShippingAddress)
                    .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

                if (order == null)
                {
                    TempData["ErrorMessage"] = "Đơn hàng không tồn tại hoặc bạn không có quyền xem.";
                    return RedirectToAction("Index", "Home");
                }

                return View(order);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải trang thành công đơn hàng");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải thông tin đơn hàng.";
                return RedirectToAction("Index", "Home");
            }
        }

        [AllowAnonymous]
        public IActionResult GetCartCount()
        {
            try
            {
                var cart = GetCartFromSession();
                var count = cart.Sum(item => item.Quantity);
                return Json(new { count = count });
            }
            catch
            {
                return Json(new { count = 0 });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Clear()
        {
            try
            {
                var cart = GetCartFromSession();
                var itemCount = cart.Sum(item => item.Quantity);

                ClearCart();

                return Json(new
                {
                    success = true,
                    message = $"Đã xóa {itemCount} sản phẩm khỏi giỏ hàng.",
                    cartItemCount = 0
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa giỏ hàng");
                return Json(new
                {
                    success = false,
                    message = "Đã xảy ra lỗi khi xóa giỏ hàng."
                });
            }
        }

        #region Private Helper Methods

        private List<CartSessionItem> GetCartFromSession()
        {
            try
            {
                var cartJson = HttpContext.Session.GetString(CART_SESSION_KEY);
                if (string.IsNullOrEmpty(cartJson))
                {
                    return new List<CartSessionItem>();
                }

                return JsonSerializer.Deserialize<List<CartSessionItem>>(cartJson) ?? new List<CartSessionItem>();
            }
            catch
            {
                return new List<CartSessionItem>();
            }
        }

        private void SaveCartToSession(List<CartSessionItem> cart)
        {
            try
            {
                var cartJson = JsonSerializer.Serialize(cart);
                HttpContext.Session.SetString(CART_SESSION_KEY, cartJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lưu giỏ hàng vào session");
            }
        }

        private void ClearCart()
        {
            HttpContext.Session.Remove(CART_SESSION_KEY);
        }

        private async Task<List<CartItemViewModel>> GetCartItemsAsync(List<CartSessionItem> cart)
        {
            if (!cart.Any())
            {
                return new List<CartItemViewModel>();
            }

            try
            {
                var productIds = cart.Select(item => item.ProductId).ToList();
                var products = await _context.Products
                    .Include(p => p.Images)
                    .Where(p => productIds.Contains(p.Id) && p.IsPublished)
                    .ToListAsync();

                var cartItems = new List<CartItemViewModel>();
                foreach (var cartItem in cart)
                {
                    var product = products.FirstOrDefault(p => p.Id == cartItem.ProductId);
                    if (product != null)
                    {
                        cartItems.Add(new CartItemViewModel
                        {
                            Product = product,
                            Quantity = cartItem.Quantity
                        });
                    }
                }

                return cartItems;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông tin sản phẩm từ giỏ hàng");
                return new List<CartItemViewModel>();
            }
        }

        private decimal CalculateCartTotal(List<CartItemViewModel> cartItems)
        {
            return cartItems.Sum(item => item.TotalPrice);
        }

        #endregion
        public async Task<IActionResult> BuyNow(int productId, int quantity = 1)
        {
            var cart = GetCartFromSession();

            var product = await _context.Products
                .Include(p => p.Images)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null || product.Stock < quantity)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại hoặc hết hàng.";
                return RedirectToAction("Index", "Products");
            }

            var existingItem = cart.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                if (existingItem.Quantity > product.Stock)
                {
                    existingItem.Quantity = product.Stock;
                }
            }
            else
            {
                cart.Add(new CartSessionItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                });
            }

            SaveCartToSession(cart);

            return RedirectToAction("Index");
        }
    }

    public class CartSessionItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedDate { get; set; }
    }
}