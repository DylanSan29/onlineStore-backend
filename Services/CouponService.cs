using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class CouponService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<Coupon>> GetAllAsync() =>
        await context.Coupons.AsNoTracking().OrderByDescending(coupon => coupon.CreatedAt).ToListAsync();

    public async Task<Coupon> CreateAsync(CouponRequest request)
    {
        ValidateRequest(request);
        var code = NormalizeCode(request.Code);
        if (await context.Coupons.AnyAsync(coupon => coupon.Code == code))
            throw new InvalidOperationException("Coupon code already exists.");

        var coupon = new Coupon
        {
            Code = code, DiscountType = NormalizeType(request.DiscountType), DiscountValue = request.DiscountValue,
            StartsAt = request.StartsAt, ExpiresAt = request.ExpiresAt, UsageLimit = request.UsageLimit
        };
        context.Coupons.Add(coupon);
        await context.SaveChangesAsync();
        return coupon;
    }

    public async Task<CouponValidationResponse> ValidateAsync(int userId, string code, decimal subtotal)
    {
        var coupon = await FindUsableAsync(userId, code);
        return new CouponValidationResponse { Code = coupon.Code, DiscountAmount = CalculateDiscount(coupon, subtotal) };
    }

    public async Task<(Coupon Coupon, decimal Discount)> GetDiscountAsync(int userId, string code, decimal subtotal)
    {
        var coupon = await FindUsableAsync(userId, code);
        return (coupon, CalculateDiscount(coupon, subtotal));
    }

    private async Task<Coupon> FindUsableAsync(int userId, string code)
    {
        var coupon = await context.Coupons.SingleOrDefaultAsync(item => item.Code == NormalizeCode(code) && item.IsActive);
        var now = DateTime.UtcNow;
        if (coupon is null || coupon.StartsAt > now || coupon.ExpiresAt <= now)
            throw new InvalidOperationException("Coupon is invalid or expired.");
        if (await context.CouponUsages.AnyAsync(usage => usage.CouponId == coupon.Id && usage.UserId == userId))
            throw new InvalidOperationException("Coupon was already used by this customer.");
        if (coupon.UsageLimit.HasValue && await context.CouponUsages.CountAsync(usage => usage.CouponId == coupon.Id) >= coupon.UsageLimit.Value)
            throw new InvalidOperationException("Coupon usage limit was reached.");
        return coupon;
    }

    private static decimal CalculateDiscount(Coupon coupon, decimal subtotal)
    {
        if (subtotal <= 0) throw new InvalidOperationException("Subtotal must be positive.");
        var discount = coupon.DiscountType == "Percentage" ? subtotal * coupon.DiscountValue / 100 : coupon.DiscountValue;
        return Math.Round(Math.Min(discount, subtotal), 2, MidpointRounding.AwayFromZero);
    }

    private static void ValidateRequest(CouponRequest request)
    {
        var type = NormalizeType(request.DiscountType);
        if (type == "Percentage" && request.DiscountValue > 100) throw new InvalidOperationException("Percentage discount cannot exceed 100.");
        if (request.ExpiresAt <= request.StartsAt) throw new InvalidOperationException("Coupon expiration must be after its start date.");
    }

    private static string NormalizeCode(string code) => code.Trim().ToUpperInvariant();
    private static string NormalizeType(string type) => type.Trim() switch
    {
        "Percentage" => "Percentage", "Fixed" => "Fixed", _ => throw new InvalidOperationException("Discount type must be Percentage or Fixed.")
    };
}
