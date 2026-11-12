using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnlineStoreBackend.Data;

#nullable disable

namespace OnlineStoreBackend.Migrations;

[DbContext(typeof(OnlineStoreContext))]
[Migration("20261106020000_AddNonPaymentTables")]
public partial class AddNonPaymentTables : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Inventories",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ProductId = table.Column<int>(type: "int", nullable: false),
                QuantityOnHand = table.Column<int>(type: "int", nullable: false),
                QuantityReserved = table.Column<int>(type: "int", nullable: false),
                ReorderLevel = table.Column<int>(type: "int", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Inventories", inventory => inventory.Id);
                table.ForeignKey("FK_Inventories_Products_ProductId", inventory => inventory.ProductId, "Products", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.Sql("INSERT INTO [Inventories] ([ProductId], [QuantityOnHand], [QuantityReserved], [ReorderLevel], [UpdatedAt]) SELECT [Id], [Quantity], 0, 5, GETUTCDATE() FROM [Products]");

        migrationBuilder.CreateTable(
            name: "RefreshTokens",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                TokenHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                RevokedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_RefreshTokens", token => token.Id);
                table.ForeignKey("FK_RefreshTokens_Users_UserId", token => token.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "PasswordResetTokens",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                TokenHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_PasswordResetTokens", token => token.Id);
                table.ForeignKey("FK_PasswordResetTokens_Users_UserId", token => token.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "EmailVerificationTokens",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                TokenHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                VerifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EmailVerificationTokens", token => token.Id);
                table.ForeignKey("FK_EmailVerificationTokens_Users_UserId", token => token.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Shipments",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                OrderId = table.Column<int>(type: "int", nullable: false),
                Carrier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                ShippedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Shipments", shipment => shipment.Id);
                table.ForeignKey("FK_Shipments_Orders_OrderId", shipment => shipment.OrderId, "Orders", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "ProductImages",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ProductId = table.Column<int>(type: "int", nullable: false),
                ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                AltText = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                SortOrder = table.Column<int>(type: "int", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductImages", image => image.Id);
                table.ForeignKey("FK_ProductImages_Products_ProductId", image => image.ProductId, "Products", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Reviews",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                ProductId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                Rating = table.Column<int>(type: "int", nullable: false),
                Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IsApproved = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Reviews", review => review.Id);
                table.ForeignKey("FK_Reviews_Products_ProductId", review => review.ProductId, "Products", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_Reviews_Users_UserId", review => review.UserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "Wishlists",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Wishlists", wishlist => wishlist.Id);
                table.ForeignKey("FK_Wishlists_Users_UserId", wishlist => wishlist.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Coupons",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                Code = table.Column<string>(type: "nvarchar(450)", nullable: false),
                DiscountType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                DiscountValue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                UsageLimit = table.Column<int>(type: "int", nullable: true),
                IsActive = table.Column<bool>(type: "bit", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Coupons", coupon => coupon.Id));

        migrationBuilder.CreateTable(
            name: "Notifications",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: false),
                Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                IsRead = table.Column<bool>(type: "bit", nullable: false),
                ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Notifications", notification => notification.Id);
                table.ForeignKey("FK_Notifications_Users_UserId", notification => notification.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "AuditLogs",
            columns: table => new
            {
                Id = table.Column<long>(type: "bigint", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                UserId = table.Column<int>(type: "int", nullable: true),
                Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EntityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                EntityId = table.Column<int>(type: "int", nullable: true),
                Details = table.Column<string>(type: "nvarchar(max)", nullable: true),
                IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_AuditLogs", log => log.Id);
                table.ForeignKey("FK_AuditLogs_Users_UserId", log => log.UserId, "Users", "Id", onDelete: ReferentialAction.SetNull);
            });

        migrationBuilder.CreateTable(
            name: "WishlistItems",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                WishlistId = table.Column<int>(type: "int", nullable: false),
                ProductId = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_WishlistItems", item => item.Id);
                table.ForeignKey("FK_WishlistItems_Wishlists_WishlistId", item => item.WishlistId, "Wishlists", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_WishlistItems_Products_ProductId", item => item.ProductId, "Products", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateTable(
            name: "CouponUsages",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false).Annotation("SqlServer:Identity", "1, 1"),
                CouponId = table.Column<int>(type: "int", nullable: false),
                UserId = table.Column<int>(type: "int", nullable: false),
                OrderId = table.Column<int>(type: "int", nullable: false),
                UsedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CouponUsages", usage => usage.Id);
                table.ForeignKey("FK_CouponUsages_Coupons_CouponId", usage => usage.CouponId, "Coupons", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_CouponUsages_Users_UserId", usage => usage.UserId, "Users", "Id", onDelete: ReferentialAction.Restrict);
                table.ForeignKey("FK_CouponUsages_Orders_OrderId", usage => usage.OrderId, "Orders", "Id", onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex("IX_Inventories_ProductId", "Inventories", "ProductId", unique: true);
        migrationBuilder.CreateIndex("IX_RefreshTokens_TokenHash", "RefreshTokens", "TokenHash", unique: true);
        migrationBuilder.CreateIndex("IX_PasswordResetTokens_TokenHash", "PasswordResetTokens", "TokenHash", unique: true);
        migrationBuilder.CreateIndex("IX_EmailVerificationTokens_TokenHash", "EmailVerificationTokens", "TokenHash", unique: true);
        migrationBuilder.CreateIndex("IX_Shipments_OrderId", "Shipments", "OrderId", unique: true);
        migrationBuilder.CreateIndex("IX_ProductImages_ProductId", "ProductImages", "ProductId");
        migrationBuilder.CreateIndex("IX_Reviews_ProductId_UserId", "Reviews", new[] { "ProductId", "UserId" }, unique: true);
        migrationBuilder.CreateIndex("IX_Wishlists_UserId", "Wishlists", "UserId", unique: true);
        migrationBuilder.CreateIndex("IX_WishlistItems_WishlistId_ProductId", "WishlistItems", new[] { "WishlistId", "ProductId" }, unique: true);
        migrationBuilder.CreateIndex("IX_Coupons_Code", "Coupons", "Code", unique: true);
        migrationBuilder.CreateIndex("IX_CouponUsages_CouponId_UserId_OrderId", "CouponUsages", new[] { "CouponId", "UserId", "OrderId" }, unique: true);
        migrationBuilder.CreateIndex("IX_Notifications_UserId_IsRead", "Notifications", new[] { "UserId", "IsRead" });
        migrationBuilder.CreateIndex("IX_AuditLogs_CreatedAt", "AuditLogs", "CreatedAt");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable("CouponUsages");
        migrationBuilder.DropTable("WishlistItems");
        migrationBuilder.DropTable("AuditLogs");
        migrationBuilder.DropTable("Notifications");
        migrationBuilder.DropTable("Coupons");
        migrationBuilder.DropTable("Wishlists");
        migrationBuilder.DropTable("Reviews");
        migrationBuilder.DropTable("ProductImages");
        migrationBuilder.DropTable("Shipments");
        migrationBuilder.DropTable("EmailVerificationTokens");
        migrationBuilder.DropTable("PasswordResetTokens");
        migrationBuilder.DropTable("RefreshTokens");
        migrationBuilder.DropTable("Inventories");
    }
}
