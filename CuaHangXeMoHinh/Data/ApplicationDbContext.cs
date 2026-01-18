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
            new Category { Id = 4, Name = "Racing (GT/F1)", Description = "Xe đua chuyên nghiệp từ các giải đấu danh giá." },
            new Category { Id = 5, Name = "Luxury SUVs", Description = "Sự kết hợp giữa sang trọng và khả năng địa hình." }
            );

            // --- Seed Brands ---
            builder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Ferrari", Description = "Huyền thoại ngựa chồm từ Maranello, Ý." },
                new Brand { Id = 2, Name = "Lamborghini", Description = "Biểu tượng bò tót hung dữ, thiết kế góc cạnh." },
                new Brand { Id = 3, Name = "Porsche", Description = "Sự chính xác và hoàn hảo từ cơ khí Đức." },
                new Brand { Id = 4, Name = "McLaren", Description = "Công nghệ đường đua F1 áp dụng cho siêu xe đường phố." },
                new Brand { Id = 5, Name = "Bugatti", Description = "Đỉnh cao của tốc độ và sự xa xỉ tột bậc." },
                new Brand { Id = 6, Name = "Ford", Description = "Sức mạnh Mỹ với dòng Mustang huyền thoại." },
                new Brand { Id = 7, Name = "Mercedes-AMG", Description = "Hiệu suất cao kết hợp sự sang trọng tuyệt đối." }
            );

            // --- Seed Products (20 items) ---
            builder.Entity<Product>().HasData(
                // Category 1 — Supercars
                new Product { Id = 1, Name = "Ferrari 488 GTB Red", Description = "Mô hình tĩnh tỷ lệ 1:18, chi tiết động cơ V8 Twin Turbo.", Price = 5500000m, Cost = 3500000m, IsPublished = true, Stock = 10, CreatedAt = DateTime.UtcNow, CategoryId = 1, BrandId = 1 },
                new Product { Id = 2, Name = "Lamborghini Aventador SVJ", Description = "Phiên bản giới hạn SVJ, màu xanh Verde Mantis, mở được cửa cắt kéo.", Price = 7200000m, Cost = 5000000m, IsPublished = true, Stock = 5, CreatedAt = DateTime.UtcNow, CategoryId = 1, BrandId = 2 },
                new Product { Id = 3, Name = "McLaren 720S Orange", Description = "Siêu xe Anh Quốc với thiết kế khí động học, màu cam Papaya Spark.", Price = 6800000m, Cost = 4200000m, IsPublished = true, Stock = 8, CreatedAt = DateTime.UtcNow, CategoryId = 1, BrandId = 4 },
                new Product { Id = 4, Name = "Porsche 911 GT3 RS", Description = "Ông vua đường đua, màu vàng Racing Yellow, chi tiết cánh gió carbon.", Price = 6000000m, Cost = 4000000m, IsPublished = true, Stock = 12, CreatedAt = DateTime.UtcNow, CategoryId = 1, BrandId = 3 },
                new Product { Id = 5, Name = "Bugatti Chiron Sport", Description = "Tuyệt tác tốc độ 1500 mã lực, bản mô hình 1:18 cực nét.", Price = 9500000m, Cost = 7000000m, IsPublished = true, Stock = 3, CreatedAt = DateTime.UtcNow, CategoryId = 1, BrandId = 5 },

                // Category 2 — Classic
                new Product { Id = 6, Name = "Ferrari 250 GTO 1962", Description = "Chiếc Ferrari đắt nhất thế giới, mô hình cổ điển cực hiếm.", Price = 12000000m, Cost = 9000000m, IsPublished = true, Stock = 2, CreatedAt = DateTime.UtcNow, CategoryId = 2, BrandId = 1 },
                new Product { Id = 7, Name = "Mercedes-Benz 300SL Gullwing", Description = "Huyền thoại cửa cánh chim, màu bạc kim loại.", Price = 8500000m, Cost = 6000000m, IsPublished = true, Stock = 4, CreatedAt = DateTime.UtcNow, CategoryId = 2, BrandId = 7 },
                new Product { Id = 8, Name = "Lamborghini Miura P400", Description = "Siêu xe đầu tiên của thế giới, thiết kế Gandini tuyệt đẹp.", Price = 7800000m, Cost = 5500000m, IsPublished = true, Stock = 6, CreatedAt = DateTime.UtcNow, CategoryId = 2, BrandId = 2 },
                new Product { Id = 9, Name = "Ford GT40 MKII 1966", Description = "Nhà vô địch Le Mans, đánh bại Ferrari, màu đen số 2.", Price = 6500000m, Cost = 4500000m, IsPublished = true, Stock = 7, CreatedAt = DateTime.UtcNow, CategoryId = 2, BrandId = 6 },

                // Category 3 — Muscle
                new Product { Id = 10, Name = "Ford Mustang Shelby GT500", Description = "Cobra chúa, màu xanh sọc trắng đặc trưng, động cơ chi tiết.", Price = 4500000m, Cost = 3000000m, IsPublished = true, Stock = 15, CreatedAt = DateTime.UtcNow, CategoryId = 3, BrandId = 6 },
                new Product { Id = 11, Name = "Chevrolet Camaro ZL1", Description = "Đối thủ truyền kiếp của Mustang, màu vàng Bumblebee.", Price = 4200000m, Cost = 2800000m, IsPublished = true, Stock = 18, CreatedAt = DateTime.UtcNow, CategoryId = 3, BrandId = 6 }, // Using Ford Brand loosely for now or adding Chevy later

                // Category 4 — Racing
                new Product { Id = 12, Name = "Ferrari F1-75 Leclerec", Description = "Xe đua F1 mùa giải 2022, số 16 Charles Leclerc.", Price = 3500000m, Cost = 2000000m, IsPublished = true, Stock = 20, CreatedAt = DateTime.UtcNow, CategoryId = 4, BrandId = 1 },
                new Product { Id = 13, Name = "Mercedes-AMG F1 W13", Description = "Xe đua F1 của Lewis Hamilton, chi tiết a-rô cực đỉnh.", Price = 3500000m, Cost = 2000000m, IsPublished = true, Stock = 20, CreatedAt = DateTime.UtcNow, CategoryId = 4, BrandId = 7 },
                new Product { Id = 14, Name = "Porsche 919 Hybrid Le Mans", Description = "Nhà vô địch Le Mans LMP1, công nghệ Hybrid tiên tiến.", Price = 5000000m, Cost = 3500000m, IsPublished = true, Stock = 5, CreatedAt = DateTime.UtcNow, CategoryId = 4, BrandId = 3 },
                new Product { Id = 15, Name = "McLaren Senna GTR", Description = "Quái vật đường đua, cánh gió khổng lồ, phiên bản giới hạn.", Price = 8000000m, Cost = 6000000m, IsPublished = true, Stock = 3, CreatedAt = DateTime.UtcNow, CategoryId = 4, BrandId = 4 },

                // Category 5 — Luxury SUVs
                new Product { Id = 16, Name = "Lamborghini Urus Yellow", Description = "Siêu SUV nhanh nhất thế giới, màu vàng Giallo Auge.", Price = 5800000m, Cost = 4000000m, IsPublished = true, Stock = 10, CreatedAt = DateTime.UtcNow, CategoryId = 5, BrandId = 2 },
                new Product { Id = 17, Name = "Mercedes-AMG G63", Description = "Ông vua địa hình, màu đen mờ Matte Black cực ngầu.", Price = 6200000m, Cost = 4500000m, IsPublished = true, Stock = 8, CreatedAt = DateTime.UtcNow, CategoryId = 5, BrandId = 7 },
                new Product { Id = 18, Name = "Rolls-Royce Cullinan", Description = "SUV sang trọng nhất thế giới, màu trắng Diamond.", Price = 10500000m, Cost = 8000000m, IsPublished = true, Stock = 2, CreatedAt = DateTime.UtcNow, CategoryId = 5, BrandId = 7 }, // Using Merc ID loosely or need new brand
                new Product { Id = 19, Name = "Porsche Cayenne Turbo GT", Description = "SUV hiệu suất cao, phá kỷ lục Nurburgring.", Price = 5500000m, Cost = 3800000m, IsPublished = true, Stock = 12, CreatedAt = DateTime.UtcNow, CategoryId = 5, BrandId = 3 },
                new Product { Id = 20, Name = "Ferrari Purosangue", Description = "FUV đầu tiên của Ferrari, thiết kế đột phá, động cơ V12.", Price = 8800000m, Cost = 6000000m, IsPublished = true, Stock = 4, CreatedAt = DateTime.UtcNow, CategoryId = 5, BrandId = 1 }
            );

            // --- Seed ProductImages (Unsplash placeholders) ---
            builder.Entity<ProductImage>().HasData(
                new ProductImage { Id = 1, ProductId = 1, Url = "https://images.unsplash.com/photo-1592758219635-aa81597d7301?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 2, ProductId = 2, Url = "https://images.unsplash.com/photo-1621135802920-133df287f89c?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 3, ProductId = 3, Url = "https://images.unsplash.com/photo-1606161474261-0f796d13bd69?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 4, ProductId = 4, Url = "https://images.unsplash.com/photo-1503376763036-066120622c74?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 5, ProductId = 5, Url = "https://images.unsplash.com/photo-1626668893632-6f3d446f4e3e?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 6, ProductId = 6, Url = "https://images.unsplash.com/photo-1582025171731-137a28e833da?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 7, ProductId = 7, Url = "https://images.unsplash.com/photo-1579737151121-6677f5258e7b?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 8, ProductId = 8, Url = "https://images.unsplash.com/photo-1566008885218-40af1701c36b?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 9, ProductId = 9, Url = "https://images.unsplash.com/photo-1558509355-63529d91f869?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 10, ProductId = 10, Url = "https://images.unsplash.com/photo-1588693951598-35616f731174?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 11, ProductId = 11, Url = "https://images.unsplash.com/photo-1627454820574-fb602698f1f7?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 12, ProductId = 12, Url = "https://images.unsplash.com/photo-1596700054790-da742ad70f1a?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 13, ProductId = 13, Url = "https://images.unsplash.com/photo-1618600028264-b525f61ba3be?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 14, ProductId = 14, Url = "https://images.unsplash.com/photo-1600712242805-5f78671224ba?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 15, ProductId = 15, Url = "https://images.unsplash.com/photo-1621689264669-70bd1cb0f443?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 16, ProductId = 16, Url = "https://images.unsplash.com/photo-1600016450005-72872381e4b3?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 17, ProductId = 17, Url = "https://images.unsplash.com/photo-1570733577570-5b651080313f?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 18, ProductId = 18, Url = "https://images.unsplash.com/photo-1631295868223-63265b40d9e4?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 19, ProductId = 19, Url = "https://images.unsplash.com/photo-1503376763036-066120622c74?w=800&q=80", IsPrimary = true },
                new ProductImage { Id = 20, ProductId = 20, Url = "https://images.unsplash.com/photo-1633511048897-bca865a9134a?w=800&q=80", IsPrimary = true }
            );
        }
    }
}
