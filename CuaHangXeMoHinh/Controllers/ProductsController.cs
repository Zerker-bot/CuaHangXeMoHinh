using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;

    public ProductsController(ApplicationDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public IActionResult Index(int? categoryId, string sortBy = "default", int page = 1, int pageSize = 12, string search = null, string minPrice = null,
    string maxPrice = null)
    {
        var products = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .Where(p => p.IsPublished)
            .AsQueryable();

        if (categoryId.HasValue)
        {
            products = products.Where(p => p.CategoryId == categoryId);
        }

        if (!string.IsNullOrEmpty(search))
            products = products.Where(p => p.Name != null && p.Name.Contains(search));
        decimal? minPriceN = null;
        decimal? maxPriceN = null;
        if (minPrice != null && decimal.TryParse(minPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tempMin))
        {
            minPriceN = tempMin;
        }

        if (maxPrice != null && decimal.TryParse(maxPrice, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tempMax))
        {
            maxPriceN = tempMax;
        }
        if (minPriceN.HasValue)
        {
            products = products.Where(p => p.Price >= minPriceN.Value);
        }
        if (maxPriceN.HasValue)
        {
            products = products.Where(p => p.Price <= maxPriceN.Value);
        }

        switch (sortBy)
        {
            case "price-asc":
                products = products.OrderBy(p => p.Price);
                break;
            case "price-desc":
                products = products.OrderByDescending(p => p.Price);
                break;
            case "name-asc":
                products = products.OrderBy(p => p.Name);
                break;
            case "name-desc":
                products = products.OrderByDescending(p => p.Name);
                break;
            case "rating":
                products = products
                    .OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)
                    .ThenByDescending(p => p.Reviews.Count);
                break;
            case "newest":
                products = products.OrderByDescending(p => p.CreatedAt);
                break;
            case "updated":
                products = products.OrderByDescending(p => p.UpdatedAt ?? p.CreatedAt);
                break;
            case "stock":
                products = products.OrderByDescending(p => p.Stock);
                break;
            default:
                products = products.OrderByDescending(p => p.CreatedAt);
                break;
        }

        var totalItems = products.Count();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
        var pagedProducts = products
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        string viewMode = Request.Cookies["ViewMode"] ?? "grid";
        ViewBag.ViewMode = viewMode == "list" ? "products-list" : "products-grid";
        ViewBag.CurrentViewMode = viewMode;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;
        ViewBag.SortBy = sortBy;
        ViewBag.CategoryId = categoryId;
        ViewBag.Categories = _context.Categories.ToList();
        ViewBag.Search = search;
        ViewBag.MinPrice = minPrice;
        ViewBag.MaxPrice = maxPrice;


        return View(pagedProducts);
    }
    public IActionResult Detail(int id)
    {
        var product = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images)
            .Include(p => p.Reviews)
            .ThenInclude(r => r.User)
            .FirstOrDefault(p => p.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        var relatedProducts = _context.Products
            .Include(p => p.Images)
            .Include(p => p.Category)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != product.Id && p.IsPublished)
            .Take(4)
            .ToList();

        ViewBag.RelatedProducts = relatedProducts;

        return View(product);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddReview(int productId, int rating, string content)
    {
        if (rating == 0 || string.IsNullOrWhiteSpace(content))
        {
            TempData["ErrorMessage"] = rating == 0 ? "Vui lòng chọn đánh giá" : "Nội dung không được để trống!";
            return RedirectToAction("Detail", new { id = productId });
        }
        if (!ModelState.IsValid)
        {
            return RedirectToAction("Detail", new { id = productId });
        }
        var userId = _userManager.GetUserId(User);

        var hasPurchased = await _context.OrderItems
            .Include(oi => oi.Order)
            .AnyAsync(oi => oi.ProductId == productId
                && oi.Order.UserId == userId
                && oi.Order.Status == OrderStatus.Delivered);

        if (!hasPurchased)
        {
            TempData["ErrorMessage"] = "Bạn cần mua sản phẩm này trước khi đánh giá!";
            return RedirectToAction("Detail", new { id = productId });
        }

        var review = new Review
        {
            ProductId = productId,
            UserId = userId,
            Rating = rating,
            Content = content,
            CreatedAt = DateTime.Now,
            IsApproved = false
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        TempData["SuccessMessage"] = "Đánh giá của bạn đã được gửi và đang chờ duyệt!";
        return RedirectToAction("Detail", new { id = productId });
    }
}



