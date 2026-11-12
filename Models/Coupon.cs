namespace OnlineStoreBackend.Models;

public sealed class Coupon
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public int? UsageLimit { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
