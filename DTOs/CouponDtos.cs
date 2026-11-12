using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class CouponRequest
{
    [Required, StringLength(40)] public required string Code { get; set; }
    [Required, StringLength(20)] public required string DiscountType { get; set; }
    [Range(0.01, 1_000_000)] public decimal DiscountValue { get; set; }
    public DateTime StartsAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    [Range(1, int.MaxValue)] public int? UsageLimit { get; set; }
}

public sealed class CouponValidationRequest
{
    [Required, StringLength(40)] public required string Code { get; set; }
}

public sealed class CouponValidationResponse
{
    public required string Code { get; init; }
    public decimal DiscountAmount { get; init; }
}
