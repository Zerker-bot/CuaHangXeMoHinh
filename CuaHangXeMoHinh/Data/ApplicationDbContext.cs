using CuaHangXeMoHinh.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace CuaHangXeMoHinh.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<OrderStatusLog> OrderStatusLogs { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();

                if (tableName != null && tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            builder.Entity<OrderItem>()
                           .HasKey(oi => new { oi.OrderId, oi.ProductId });

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.Items)
                .HasForeignKey(oi => oi.OrderId);

            builder.Entity<OrderItem>()
                .HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId);

            builder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.ShippingFee)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .Property(o => o.Discount)
                .HasPrecision(18, 2);

            builder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);



            builder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<OrderStatusLog>(entity =>
            {
                entity.ToTable("OrderStatusLogs");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.ChangedBy)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Note)
                    .HasMaxLength(1000);

                entity.Property(e => e.ChangedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.StatusLogs)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                },
                new IdentityRole()
                {
                    Id = "2",
                    Name = "Staff",
                    NormalizedName = "STAFF",
                },
                new IdentityRole()
                {
                    Id = "3",
                    Name = "Customer",
                    NormalizedName = "CUSTOMER",
                }
            );

            builder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Supercars", Description = "Siêu xe thể thao hiệu suất cao, thiết kế khí động học." },
            new Category { Id = 2, Name = "Classic", Description = "Những mẫu xe cổ điển mang vẻ đẹp vượt thời gian." },
            new Category { Id = 3, Name = "Muscle", Description = "Sức mạnh cơ bắp Mỹ thuần túy, động cơ V8 gầm rú." },
            new Category { Id = 4, Name = "Racing (GT/F1)", Description = "Xe đua chuyên nghiệp từ các giải đấu danh giá." }
            );

            // --- Seed Brands ---
            builder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Ferrari", Description = "Huyền thoại ngựa chồm từ Maranello, Ý." },
                new Brand { Id = 2, Name = "Lamborghini", Description = "Biểu tượng bò tót hung dữ, thiết kế góc cạnh." },
                new Brand { Id = 3, Name = "Porsche", Description = "Sự chính xác và hoàn hảo từ cơ khí Đức." },
                new Brand { Id = 4, Name = "McLaren", Description = "Công nghệ đường đua F1 áp dụng cho siêu xe đường phố." },
                new Brand { Id = 5, Name = "Bugatti", Description = "Đỉnh cao của tốc độ và sự xa xỉ tột bậc." },
                new Brand { Id = 6, Name = "Ford", Description = "Sức mạnh Mỹ với dòng Mustang huyền thoại." },
                new Brand { Id = 7, Name = "Mercedes-AMG", Description = "Hiệu suất cao kết hợp sự sang trọng tuyệt đối." },
                new Brand { Id = 8, Name = "Rolls-Royce ", Description = "Hiệu suất cao kết hợp sự sang trọng tuyệt đối." },
                new Brand { Id = 9, Name = "Chevrolet ", Description = "Chevrolet." },
                 new Brand { Id = 10, Name = "Koenigsegg  ", Description = "Koenigsegg ." },
                                 new Brand { Id =11, Name = "Pagani", Description = "Pagani" },
                                                 new Brand { Id = 12, Name = "Cadillac ", Description = "Cadillac." },
                                                                  new Brand { Id = 13, Name = "Dodge  ", Description = "Dodge ." },
                                                                                  new Brand { Id = 14, Name = "Aston Martin", Description = "Aston Martin" }





            );

            // --- Seed Products (20 items) ---
            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "McLaren 720s", Description = "Mô hình siêu xe Mclaren 720s\r\n\r\nTỉ Lệ: 1:32\r\n\r\nKích thước: Dài 15.5cm; rộng 5cm; cao 4cm\r\n\r\nChức năng: Có đèn, điện trước và sau khi mở cửa; mở được 2 cửa cánh gió; xe có trớn để chạy được.\r\n\r\nChất Liệu: Đối với xe ô tô toàn bộ khung xe được làm đúc bằng nhựa resin hoặc hợp kim nguyên khối, nội thất xe làm bằng nhựa ABS , lốp xe được làm từ cao su tự nhiên, các chi tiết ống xả được mạ crôm , lưới tản nhiệt bằng kim loại\r\n\r\nĐược sơn bằng tĩnh điện cho nước sơn sáng bóng, mỏng, bền. vì này là hàng cao cấp nên được sơn nhiều lớp theo tiêu chuẩn của nhà sản xuất.", Price = 189000, Cost = 299000, IsPublished = true, Stock = 10, CreatedAt = DateTime.UtcNow, CategoryId = 1, BrandId = 4 },
                                new Product { Id = 2, Name = "Koenigsegg Jesko ", Description = "Thông tin sản phẩm Mô hình xe Koenigsegg Jesko tỉ lệ 1:32 kèm đế trưng bày\r\n\r\nTỉ lệ: 1:32 (nhỏ hơn xe thật 32 lần)\r\n\r\nMàu: Màu cam,trắng,đen\r\n\r\nKích thước: Dài x Rộng x Cao (15 x 6,5 x 4,5 cm)\r\n\r\nKhối lượng: ~300gr\r\n\r\nChất liệu: Toàn bộ phần thân vỏ được làm bằng kim loại nguyên khối với nước sơn tĩnh điện 3 lớp cực đẹp, Gầm xe và nội thất bằng nhựa, bánh xe bằng cao su cao cấp\r\n\r\nTính năng:\r\n\r\n️ Mở 2 cửa, capo và cốp\r\n\r\n️ Có đèn âm thanh và trớn\r\n\r\nMục đích sử dụng:\r\n\r\n️ Sưu tầm xe mô hình\r\n\r\n️ Trang trí, nội thất\r\n\r\n️ Quà tặng cho bạn nam, quà tặng cho bé trai, quà noel, quà sinh nhật", Price = 189000, Cost = 289000, IsPublished = true, Stock = 10, CreatedAt = DateTime.UtcNow, CategoryId = 1, BrandId = 10 },
                                new Product { Id = 3, Name = "Cadillac Eldorado Biarritz Classic  ", Description = "🏎️ Mô Hình Cadillac Eldorado Biarritz Classic 1:32 (Hợp Kim)\r\nChất liệu: Hợp kim Diecast siêu bền, lốp cao su, nội thất ABS.\r\n\r\nKích thước: ~15.5 x 7 x 4 cm.\r\n\r\nChức năng:\r\n\r\nMở toàn bộ cửa (cánh chim), nắp máy trước/sau.\r\n\r\nPhun khói: Có hệ thống phun sương giả lập khói từ ống xả (châm nước).\r\n\r\nĐèn & Âm thanh: Đèn LED trước/sau + tiếng động cơ gầm rú.\r\n\r\nVận hành: Có cót lùi (Pull-back) và giảm xóc 4 bánh.\r\n\r\nMục đích: Quà tặng, sưu tầm, decor bàn làm việc.", Price = 199000, Cost = 299000, IsPublished = true, Stock = 10, CreatedAt = DateTime.UtcNow, CategoryId = 2, BrandId = 12 },
                                new Product { Id = 4, Name = "Dodge Challenger SRT Hellcat Redeye V8 ", Description = "🏎️ Mô Hình Dodge Challenger SRT Hellcat Redeye V8 1:32 (Hợp Kim)\r\nChất liệu: Hợp kim Diecast siêu bền, lốp cao su, nội thất ABS.\r\n\r\nKích thước: ~15.5 x 7 x 4 cm.\r\n\r\nChức năng:\r\n\r\nMở toàn bộ cửa (cánh chim), nắp máy trước/sau.\r\n\r\nPhun khói: Có hệ thống phun sương giả lập khói từ ống xả (châm nước).\r\n\r\nĐèn & Âm thanh: Đèn LED trước/sau + tiếng động cơ gầm rú.\r\n\r\nVận hành: Có cót lùi (Pull-back) và giảm xóc 4 bánh.\r\n\r\nMục đích: Quà tặng, sưu tầm, decor bàn làm việc.", Price = 199000, Cost = 299000, IsPublished = true, Stock = 10, CreatedAt = DateTime.UtcNow, CategoryId = 3, BrandId = 13 },
                                                                new Product { Id = 5, Name = " Aston Martin Redbull F1 ", Description = "🏎️ Mô Hình  Aston Martin Redbull F1V8 1:42 (Hợp Kim)\r\nChất liệu: Hợp kim Diecast siêu bền, lốp cao su, nội thất ABS.\r\n\r\nKích thước: ~15.5 x 7 x 4 cm.\r\n\r\nChức năng:\r\n\r\nMở toàn bộ cửa (cánh chim), nắp máy trước/sau.\r\n\r\nPhun khói: Có hệ thống phun sương giả lập khói từ ống xả (châm nước).\r\n\r\nĐèn & Âm thanh: Đèn LED trước/sau + tiếng động cơ gầm rú.\r\n\r\nVận hành: Có cót lùi (Pull-back) và giảm xóc 4 bánh.\r\n\r\nMục đích: Quà tặng, sưu tầm, decor bàn làm việc.", Price = 199000, Cost = 299000, IsPublished = true, Stock = 10, CreatedAt = DateTime.UtcNow, CategoryId = 4, BrandId = 14 }

            );

            // --- Seed ProductImages (Unsplash placeholders) ---
            builder.Entity<ProductImage>().HasData(

    // Supercars
    new ProductImage { Id = 1, ProductId = 1, Url = "/images/MClaren_720s/main.png", IsPrimary = true },
     new ProductImage { Id = 2, ProductId = 1, Url = "/images/MClaren_720s/1.png", IsPrimary = false },
      new ProductImage { Id = 3, ProductId = 1, Url = "/images/MClaren_720s/2.png", IsPrimary = false },
       new ProductImage { Id = 4, ProductId = 1, Url = "/images/MClaren_720s/3.png", IsPrimary = false },
        new ProductImage { Id = 5, ProductId = 1, Url = "/images/MClaren_720s/4.png", IsPrimary = false },
         new ProductImage { Id = 6, ProductId = 1, Url = "/images/MClaren_720s/5.png", IsPrimary = false },
          new ProductImage { Id = 7, ProductId = 1, Url = "/images/MClaren_720s/6.png", IsPrimary = false },
           new ProductImage { Id = 8, ProductId = 2, Url = "/images/Koenigsegg_Jesko/main.png", IsPrimary = true },
            new ProductImage { Id = 9, ProductId = 2, Url = "/images/Koenigsegg_Jesko/1.png", IsPrimary = false },
             new ProductImage { Id = 10, ProductId = 2, Url = "/images/Koenigsegg_Jesko/2.png", IsPrimary = false },
              new ProductImage { Id = 11, ProductId = 2, Url = "/images/Koenigsegg_Jesko/3.png", IsPrimary = false },
                new ProductImage { Id = 12, ProductId = 3, Url = "/images/Cadillac_Eldorado_Biarritz_Classic/main.png", IsPrimary = true },
                  new ProductImage { Id = 13, ProductId = 3, Url = "/images/Cadillac_Eldorado_Biarritz_Classic/1.png", IsPrimary = false },
                    new ProductImage { Id = 14, ProductId = 3, Url = "/images/Cadillac_Eldorado_Biarritz_Classic/2.png", IsPrimary = false },
                                        new ProductImage { Id = 15, ProductId = 4, Url = "/images/Dodge_challenger/main.png", IsPrimary = true },
                    new ProductImage { Id = 16, ProductId = 4, Url = "/images/Dodge_challenger/1.png", IsPrimary = false },
                    new ProductImage { Id = 17, ProductId = 4, Url = "/images/Dodge_challenger/2.png", IsPrimary = false },
                                        new ProductImage { Id = 18, ProductId = 5, Url = "/images/AstonMartin_rebullf1/main.png", IsPrimary = true },
                    new ProductImage { Id = 19, ProductId = 5, Url = "/images/AstonMartin_rebullf1/1.png", IsPrimary = false },
                    new ProductImage { Id = 20, ProductId = 5, Url = "/images/AstonMartin_rebullf1/2.png", IsPrimary = false },
                                        new ProductImage { Id = 21, ProductId = 5, Url = "/images/AstonMartin_rebullf1/3.png", IsPrimary = false }





);

        }
    }
}
