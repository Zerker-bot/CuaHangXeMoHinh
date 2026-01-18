using CuaHangXeMoHinh.Data;
using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CuaHangXeMoHinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Staff")]
    public class ProductImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ProductImagesController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // POST: api/ProductImages/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage([FromForm] ProductImageUploadDto dto)
        {
            try
            {
                if (dto.ProductId <= 0)
                    return BadRequest(new { success = false, message = "ProductId không hợp lệ" });

                var product = await _context.Products.FindAsync(dto.ProductId);
                if (product == null)
                    return NotFound(new { success = false, message = "Sản phẩm không tồn tại" });

                string imageUrl;

                if (dto.ImageFile != null && dto.ImageFile.Length > 0)
                {
                    if (dto.ImageFile.Length > 5 * 1024 * 1024)
                        return BadRequest(new { success = false, message = "File ảnh không được vượt quá 5MB" });

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var extension = Path.GetExtension(dto.ImageFile.FileName).ToLower();
                    if (!allowedExtensions.Contains(extension))
                        return BadRequest(new { success = false, message = "Chỉ chấp nhận file ảnh (JPG, PNG, GIF)" });

                    var fileName = $"{Guid.NewGuid()}{extension}";
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");

                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await dto.ImageFile.CopyToAsync(stream);
                    }

                    imageUrl = $"/uploads/products/{fileName}";
                }
                else if (!string.IsNullOrEmpty(dto.ImageUrl))
                {
                    imageUrl = dto.ImageUrl;
                }
                else
                {
                    return BadRequest(new { success = false, message = "Vui lòng chọn file hoặc nhập URL" });
                }

                if (dto.IsPrimary)
                {
                    var currentPrimary = await _context.ProductImages
                        .Where(x => x.ProductId == dto.ProductId && x.IsPrimary)
                        .ToListAsync();

                    foreach (var img in currentPrimary)
                    {
                        img.IsPrimary = false;
                    }
                }

                var productImage = new ProductImage
                {
                    ProductId = dto.ProductId,
                    Url = imageUrl,
                    AltText = dto.AltText,
                    IsPrimary = dto.IsPrimary,
                };

                _context.ProductImages.Add(productImage);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Thêm ảnh thành công",
                    image = new
                    {
                        id = productImage.Id,
                        url = productImage.Url,
                        altText = productImage.AltText,
                        isPrimary = productImage.IsPrimary
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi upload ảnh: " + ex.Message
                });
            }
        }

        // DELETE: api/ProductImages/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var image = await _context.ProductImages.FindAsync(id);
                if (image == null)
                    return NotFound(new { success = false, message = "Ảnh không tồn tại" });

                if (image.Url.StartsWith("/uploads/"))
                {
                    var filePath = Path.Combine(_environment.WebRootPath, image.Url.TrimStart('/'));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.ProductImages.Remove(image);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Xóa ảnh thành công"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi khi xóa ảnh: " + ex.Message
                });
            }
        }

        // POST: api/ProductImages/{id}/set-primary
        [HttpPost("{id}/set-primary")]
        public async Task<IActionResult> SetAsPrimary(int id)
        {
            try
            {
                var image = await _context.ProductImages.FindAsync(id);
                if (image == null)
                    return NotFound(new { success = false, message = "Ảnh không tồn tại" });

                var otherImages = await _context.ProductImages
                    .Where(x => x.ProductId == image.ProductId && x.Id != id)
                    .ToListAsync();

                foreach (var img in otherImages)
                {
                    img.IsPrimary = false;
                }

                image.IsPrimary = true;
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Đã đặt làm ảnh chính"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Lỗi: " + ex.Message
                });
            }
        }
    }

    public class ProductImageUploadDto
    {
        public int ProductId { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageUrl { get; set; }
        public string? AltText { get; set; }
        public bool IsPrimary { get; set; }
    }
}