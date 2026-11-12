using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Data
{
    public class OnlineStoreContext : DbContext
    {
        public OnlineStoreContext(DbContextOptions<OnlineStoreContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; } 
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<InventoryMovement> InventoryMovements { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }
        public DbSet<EmailVerificationToken> EmailVerificationTokens { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<WishlistItem> WishlistItems { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<CouponUsage> CouponUsages { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(product => product.Price).HasPrecision(18, 2);
                entity.Property(product => product.Discount).HasPrecision(18, 2);
                entity.Property(product => product.Rating).HasPrecision(3, 2);
                entity.Property(product => product.Weight).HasPrecision(18, 3);
                entity.HasIndex(product => product.SKU).IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(order => order.Subtotal).HasPrecision(18, 2);
                entity.Property(order => order.ShippingCost).HasPrecision(18, 2);
                entity.Property(order => order.Tax).HasPrecision(18, 2);
                entity.Property(order => order.Discount).HasPrecision(18, 2);
                entity.Property(order => order.Total).HasPrecision(18, 2);
                entity.HasMany(order => order.Items)
                    .WithOne(item => item.Order)
                    .HasForeignKey(item => item.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne<User>().WithMany().HasForeignKey(order => order.UserId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.Property(item => item.UnitPrice).HasPrecision(18, 2);
                entity.Property(item => item.LineTotal).HasPrecision(18, 2);
                entity.HasOne<Product>().WithMany().HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<InventoryMovement>().HasOne<Product>().WithMany()
                .HasForeignKey(movement => movement.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Address>().HasOne<User>().WithMany()
                .HasForeignKey(address => address.UserId).OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Inventory>(entity =>
            {
                entity.HasIndex(inventory => inventory.ProductId).IsUnique();
                entity.HasOne(inventory => inventory.Product).WithOne(product => product.Inventory)
                    .HasForeignKey<Inventory>(inventory => inventory.ProductId).OnDelete(DeleteBehavior.Restrict);
            });
            modelBuilder.Entity<RefreshToken>().HasIndex(token => token.TokenHash).IsUnique();
            modelBuilder.Entity<PasswordResetToken>().HasIndex(token => token.TokenHash).IsUnique();
            modelBuilder.Entity<EmailVerificationToken>().HasIndex(token => token.TokenHash).IsUnique();
            modelBuilder.Entity<Shipment>().HasIndex(shipment => shipment.OrderId).IsUnique();
            modelBuilder.Entity<ProductImage>().HasIndex(image => image.ProductId);
            modelBuilder.Entity<Review>().HasIndex(review => new { review.ProductId, review.UserId }).IsUnique();
            modelBuilder.Entity<Wishlist>().HasIndex(wishlist => wishlist.UserId).IsUnique();
            modelBuilder.Entity<WishlistItem>().HasIndex(item => new { item.WishlistId, item.ProductId }).IsUnique();
            modelBuilder.Entity<Coupon>().HasIndex(coupon => coupon.Code).IsUnique();
            modelBuilder.Entity<CouponUsage>().HasIndex(usage => new { usage.CouponId, usage.UserId, usage.OrderId }).IsUnique();
            modelBuilder.Entity<Notification>().HasIndex(notification => new { notification.UserId, notification.IsRead });
            modelBuilder.Entity<AuditLog>().HasIndex(log => log.CreatedAt);

            modelBuilder.Entity<Shipment>().HasOne<Order>().WithOne().HasForeignKey<Shipment>(shipment => shipment.OrderId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ProductImage>().HasOne<Product>().WithMany().HasForeignKey(image => image.ProductId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Review>().HasOne<Product>().WithMany().HasForeignKey(review => review.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Review>().HasOne<User>().WithMany().HasForeignKey(review => review.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Wishlist>().HasOne<User>().WithMany().HasForeignKey(wishlist => wishlist.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WishlistItem>().HasOne<Wishlist>().WithMany(wishlist => wishlist.Items).HasForeignKey(item => item.WishlistId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<WishlistItem>().HasOne<Product>().WithMany().HasForeignKey(item => item.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Coupon>().Property(coupon => coupon.DiscountValue).HasPrecision(18, 2);
            modelBuilder.Entity<CouponUsage>().HasOne<Coupon>().WithMany().HasForeignKey(usage => usage.CouponId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CouponUsage>().HasOne<User>().WithMany().HasForeignKey(usage => usage.UserId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<CouponUsage>().HasOne<Order>().WithMany().HasForeignKey(usage => usage.OrderId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Notification>().HasOne<User>().WithMany().HasForeignKey(notification => notification.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<AuditLog>().HasOne<User>().WithMany().HasForeignKey(log => log.UserId).OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<RefreshToken>().HasOne<User>().WithMany().HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PasswordResetToken>().HasOne<User>().WithMany().HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<EmailVerificationToken>().HasOne<User>().WithMany().HasForeignKey(token => token.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
