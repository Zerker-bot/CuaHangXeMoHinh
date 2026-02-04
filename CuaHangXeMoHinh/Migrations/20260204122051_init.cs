using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CuaHangXeMoHinh.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CategoryId = table.Column<int>(type: "int", nullable: false),
                    BrandId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Line1 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Line2 = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Province = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Addresses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogins_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_UserTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductImages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AltText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductImages_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Response = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ShippingAddressId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ShippingFee = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Addresses_ShippingAddressId",
                        column: x => x.ShippingAddressId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => new { x.OrderId, x.ProductId });
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderStatusLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ChangedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    Note = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderStatusLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderStatusLogs_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Huyền thoại ngựa chồm từ Maranello, Ý.", "Ferrari" },
                    { 2, "Biểu tượng bò tót hung dữ, thiết kế góc cạnh.", "Lamborghini" },
                    { 3, "Sự chính xác và hoàn hảo từ cơ khí Đức.", "Porsche" },
                    { 4, "Công nghệ đường đua F1 áp dụng cho siêu xe đường phố.", "McLaren" },
                    { 5, "Đỉnh cao của tốc độ và sự xa xỉ tột bậc.", "Bugatti" },
                    { 6, "Sức mạnh Mỹ với dòng Mustang huyền thoại.", "Ford" },
                    { 7, "Hiệu suất cao kết hợp sự sang trọng tuyệt đối.", "Mercedes-AMG" },
                    { 8, "Hiệu suất cao kết hợp sự sang trọng tuyệt đối.", "Rolls-Royce " },
                    { 9, "Chevrolet.", "Chevrolet " },
                    { 10, "Koenigsegg .", "Koenigsegg  " },
                    { 11, "Pagani", "Pagani" },
                    { 12, "Cadillac.", "Cadillac " },
                    { 13, "Dodge .", "Dodge  " },
                    { 14, "Aston Martin", "Aston Martin" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "Siêu xe thể thao hiệu suất cao, thiết kế khí động học.", "Supercars" },
                    { 2, "Những mẫu xe cổ điển mang vẻ đẹp vượt thời gian.", "Classic" },
                    { 3, "Sức mạnh cơ bắp Mỹ thuần túy, động cơ V8 gầm rú.", "Muscle" },
                    { 4, "Xe đua chuyên nghiệp từ các giải đấu danh giá.", "Racing (GT/F1)" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1", null, "Admin", "ADMIN" },
                    { "2", null, "Staff", "STAFF" },
                    { "3", null, "Customer", "CUSTOMER" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "BrandId", "CategoryId", "Cost", "CreatedAt", "Description", "IsPublished", "Name", "Price", "Stock", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 4, 1, 299000m, new DateTime(2026, 2, 4, 12, 20, 50, 582, DateTimeKind.Utc).AddTicks(2665), "Mô hình siêu xe Mclaren 720s\r\n\r\nTỉ Lệ: 1:32\r\n\r\nKích thước: Dài 15.5cm; rộng 5cm; cao 4cm\r\n\r\nChức năng: Có đèn, điện trước và sau khi mở cửa; mở được 2 cửa cánh gió; xe có trớn để chạy được.\r\n\r\nChất Liệu: Đối với xe ô tô toàn bộ khung xe được làm đúc bằng nhựa resin hoặc hợp kim nguyên khối, nội thất xe làm bằng nhựa ABS , lốp xe được làm từ cao su tự nhiên, các chi tiết ống xả được mạ crôm , lưới tản nhiệt bằng kim loại\r\n\r\nĐược sơn bằng tĩnh điện cho nước sơn sáng bóng, mỏng, bền. vì này là hàng cao cấp nên được sơn nhiều lớp theo tiêu chuẩn của nhà sản xuất.", true, "McLaren 720s", 189000m, 10, null },
                    { 2, 10, 1, 289000m, new DateTime(2026, 2, 4, 12, 20, 50, 582, DateTimeKind.Utc).AddTicks(2669), "Thông tin sản phẩm Mô hình xe Koenigsegg Jesko tỉ lệ 1:32 kèm đế trưng bày\r\n\r\nTỉ lệ: 1:32 (nhỏ hơn xe thật 32 lần)\r\n\r\nMàu: Màu cam,trắng,đen\r\n\r\nKích thước: Dài x Rộng x Cao (15 x 6,5 x 4,5 cm)\r\n\r\nKhối lượng: ~300gr\r\n\r\nChất liệu: Toàn bộ phần thân vỏ được làm bằng kim loại nguyên khối với nước sơn tĩnh điện 3 lớp cực đẹp, Gầm xe và nội thất bằng nhựa, bánh xe bằng cao su cao cấp\r\n\r\nTính năng:\r\n\r\n️ Mở 2 cửa, capo và cốp\r\n\r\n️ Có đèn âm thanh và trớn\r\n\r\nMục đích sử dụng:\r\n\r\n️ Sưu tầm xe mô hình\r\n\r\n️ Trang trí, nội thất\r\n\r\n️ Quà tặng cho bạn nam, quà tặng cho bé trai, quà noel, quà sinh nhật", true, "Koenigsegg Jesko ", 189000m, 10, null },
                    { 3, 12, 2, 299000m, new DateTime(2026, 2, 4, 12, 20, 50, 582, DateTimeKind.Utc).AddTicks(2671), "🏎️ Mô Hình Cadillac Eldorado Biarritz Classic 1:32 (Hợp Kim)\r\nChất liệu: Hợp kim Diecast siêu bền, lốp cao su, nội thất ABS.\r\n\r\nKích thước: ~15.5 x 7 x 4 cm.\r\n\r\nChức năng:\r\n\r\nMở toàn bộ cửa (cánh chim), nắp máy trước/sau.\r\n\r\nPhun khói: Có hệ thống phun sương giả lập khói từ ống xả (châm nước).\r\n\r\nĐèn & Âm thanh: Đèn LED trước/sau + tiếng động cơ gầm rú.\r\n\r\nVận hành: Có cót lùi (Pull-back) và giảm xóc 4 bánh.\r\n\r\nMục đích: Quà tặng, sưu tầm, decor bàn làm việc.", true, "Cadillac Eldorado Biarritz Classic  ", 199000m, 10, null },
                    { 4, 13, 3, 299000m, new DateTime(2026, 2, 4, 12, 20, 50, 582, DateTimeKind.Utc).AddTicks(2674), "🏎️ Mô Hình Dodge Challenger SRT Hellcat Redeye V8 1:32 (Hợp Kim)\r\nChất liệu: Hợp kim Diecast siêu bền, lốp cao su, nội thất ABS.\r\n\r\nKích thước: ~15.5 x 7 x 4 cm.\r\n\r\nChức năng:\r\n\r\nMở toàn bộ cửa (cánh chim), nắp máy trước/sau.\r\n\r\nPhun khói: Có hệ thống phun sương giả lập khói từ ống xả (châm nước).\r\n\r\nĐèn & Âm thanh: Đèn LED trước/sau + tiếng động cơ gầm rú.\r\n\r\nVận hành: Có cót lùi (Pull-back) và giảm xóc 4 bánh.\r\n\r\nMục đích: Quà tặng, sưu tầm, decor bàn làm việc.", true, "Dodge Challenger SRT Hellcat Redeye V8 ", 199000m, 10, null },
                    { 5, 14, 4, 299000m, new DateTime(2026, 2, 4, 12, 20, 50, 582, DateTimeKind.Utc).AddTicks(2676), "🏎️ Mô Hình  Aston Martin Redbull F1V8 1:42 (Hợp Kim)\r\nChất liệu: Hợp kim Diecast siêu bền, lốp cao su, nội thất ABS.\r\n\r\nKích thước: ~15.5 x 7 x 4 cm.\r\n\r\nChức năng:\r\n\r\nMở toàn bộ cửa (cánh chim), nắp máy trước/sau.\r\n\r\nPhun khói: Có hệ thống phun sương giả lập khói từ ống xả (châm nước).\r\n\r\nĐèn & Âm thanh: Đèn LED trước/sau + tiếng động cơ gầm rú.\r\n\r\nVận hành: Có cót lùi (Pull-back) và giảm xóc 4 bánh.\r\n\r\nMục đích: Quà tặng, sưu tầm, decor bàn làm việc.", true, " Aston Martin Redbull F1 ", 199000m, 10, null }
                });

            migrationBuilder.InsertData(
                table: "ProductImages",
                columns: new[] { "Id", "AltText", "IsPrimary", "ProductId", "Url" },
                values: new object[,]
                {
                    { 1, null, true, 1, "/images/MClaren_720s/main.png" },
                    { 2, null, false, 1, "/images/MClaren_720s/1.png" },
                    { 3, null, false, 1, "/images/MClaren_720s/2.png" },
                    { 4, null, false, 1, "/images/MClaren_720s/3.png" },
                    { 5, null, false, 1, "/images/MClaren_720s/4.png" },
                    { 6, null, false, 1, "/images/MClaren_720s/5.png" },
                    { 7, null, false, 1, "/images/MClaren_720s/6.png" },
                    { 8, null, true, 2, "/images/Koenigsegg_Jesko/main.png" },
                    { 9, null, false, 2, "/images/Koenigsegg_Jesko/1.png" },
                    { 10, null, false, 2, "/images/Koenigsegg_Jesko/2.png" },
                    { 11, null, false, 2, "/images/Koenigsegg_Jesko/3.png" },
                    { 12, null, true, 3, "/images/Cadillac_Eldorado_Biarritz_Classic/main.png" },
                    { 13, null, false, 3, "/images/Cadillac_Eldorado_Biarritz_Classic/1.png" },
                    { 14, null, false, 3, "/images/Cadillac_Eldorado_Biarritz_Classic/2.png" },
                    { 15, null, true, 4, "/images/Dodge_challenger/main.png" },
                    { 16, null, false, 4, "/images/Dodge_challenger/1.png" },
                    { 17, null, false, 4, "/images/Dodge_challenger/2.png" },
                    { 18, null, true, 5, "/images/AstonMartin_rebullf1/main.png" },
                    { 19, null, false, 5, "/images/AstonMartin_rebullf1/1.png" },
                    { 20, null, false, 5, "/images/AstonMartin_rebullf1/2.png" },
                    { 21, null, false, 5, "/images/AstonMartin_rebullf1/3.png" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserId",
                table: "Addresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductId",
                table: "OrderItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressId",
                table: "Orders",
                column: "ShippingAddressId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderStatusLogs_OrderId",
                table: "OrderStatusLogs",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ProductId",
                table: "ProductImages",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandId",
                table: "Products",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ProductId",
                table: "Reviews",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleClaims_RoleId",
                table: "RoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLogins_UserId",
                table: "UserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "Users",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderStatusLogs");

            migrationBuilder.DropTable(
                name: "ProductImages");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "RoleClaims");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserLogins");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTokens");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
