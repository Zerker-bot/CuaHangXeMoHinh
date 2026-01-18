using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CuaHangXeMoHinh.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Staff")]

    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index(string searchString,
            int? categoryId,
            int? brandId,
            bool? isPublished,
            decimal? minPrice,
            decimal? maxPrice,
            int? minStock,
            int? maxStock,
            string sortBy = "name",
            string sortOrder = "asc")
        {
            var query = _context.Products
                 .Include(p => p.Brand)
                 .Include(p => p.Category)
                 .AsQueryable();

            // Tìm kiếm theo tên
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(p =>
                    p.Name.Contains(searchString) ||
                    p.Description.Contains(searchString));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue && categoryId > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Lọc theo thương hiệu
            if (brandId.HasValue && brandId > 0)
            {
                query = query.Where(p => p.BrandId == brandId);
            }

            // Lọc theo trạng thái
            if (isPublished.HasValue)
            {
                query = query.Where(p => p.IsPublished == isPublished);
            }

            // Lọc theo khoảng giá
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            // Lọc theo khoảng tồn kho
            if (minStock.HasValue)
            {
                query = query.Where(p => p.Stock >= minStock);
            }

            if (maxStock.HasValue)
            {
                query = query.Where(p => p.Stock <= maxStock);
            }

            // Sắp xếp
            query = sortBy.ToLower() switch
            {
                "price" => sortOrder == "asc"
                    ? query.OrderBy(p => p.Price)
                    : query.OrderByDescending(p => p.Price),
                "stock" => sortOrder == "asc"
                    ? query.OrderBy(p => p.Stock)
                    : query.OrderByDescending(p => p.Stock),
                "date" => sortOrder == "asc"
                    ? query.OrderBy(p => p.CreatedAt)
                    : query.OrderByDescending(p => p.CreatedAt),
                _ => sortOrder == "asc"
                    ? query.OrderBy(p => p.Name)
                    : query.OrderByDescending(p => p.Name),
            };

            var products = await query.ToListAsync();

            var categories = await _context.Categories.ToListAsync();
            var brands = await _context.Brands.ToListAsync();

            ViewData["CategoryList"] = new SelectList(categories, "Id", "Name", categoryId);
            ViewData["BrandList"] = new SelectList(brands, "Id", "Name", brandId);

            ViewBag.SearchString = searchString;
            ViewBag.CategoryId = categoryId;
            ViewBag.BrandId = brandId;
            ViewBag.IsPublished = isPublished;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.MinStock = minStock;
            ViewBag.MaxStock = maxStock;
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;

            ViewBag.TotalProducts = products.Count;
            ViewBag.TotalValue = products.Sum(p => p.Price * p.Stock);
            ViewBag.AveragePrice = products.Any() ? products.Average(p => p.Price) : 0;
            ViewBag.LowStockCount = products.Count(p => p.Stock > 0 && p.Stock <= 10);
            ViewBag.OutOfStockCount = products.Count(p => p.Stock == 0);

            return View(products);
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name");
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Cost,IsPublished,Stock,CreatedAt,UpdatedAt,CategoryId,BrandId")] Product product)
        {

            if (ModelState.IsValid)
            {
                product.CreatedAt = DateTime.Now;
                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Đã thêm sản phẩm {product.Name} thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
         .Include(p => p.Images)
         .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Cost,IsPublished,Stock,CreatedAt,UpdatedAt,CategoryId,BrandId")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.UpdatedAt = DateTime.Now;
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = $"Đã cập nhật sản phẩm {product.Name} thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["BrandId"] = new SelectList(_context.Brands, "Id", "Name", product.BrandId);
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
        .Include(p => p.Reviews)
        .Include(p => p.Images)
        .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return NotFound();
            }

            var hasOrders = await _context.OrderItems.AnyAsync(oi => oi.ProductId == id);
            if (hasOrders)
            {
                TempData["ErrorMessage"] = $"Không thể xóa sản phẩm {product.Name} vì đã có đơn hàng liên quan!";
                return RedirectToAction(nameof(Index));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã xóa sản phẩm {product.Name} thành công!";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePublish(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm!";
                return RedirectToAction(nameof(Index));
            }

            product.IsPublished = !product.IsPublished;
            product.UpdatedAt = DateTime.UtcNow;
            _context.Update(product);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Đã {(product.IsPublished ? "công khai" : "ẩn")} sản phẩm '{product.Name}'!";
            return RedirectToAction(nameof(Index));
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
        [HttpGet("Admin/Products/GetProductImages/{productId}")]
        public async Task<IActionResult> GetProductImages(int productId)
        {
            try
            {
                var images = await _context.ProductImages
                    .Where(x => x.ProductId == productId)
                    .ToListAsync();

                if (images == null || !images.Any())
                {
                    return PartialView("_ProductImagesPartial", new List<ProductImage>());
                }

                return PartialView("_ProductImagesPartial", images);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Lỗi khi tải ảnh");
            }

        }

    }
}
