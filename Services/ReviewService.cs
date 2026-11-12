using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class ReviewService(OnlineStoreContext context, AuditLogService? audit = null)
{
    public async Task<IReadOnlyList<ReviewResponse>> GetPendingAsync() =>
        await context.Reviews.AsNoTracking().Where(review => !review.IsApproved)
            .OrderBy(review => review.CreatedAt).Select(review => new ReviewResponse
            {
                Id = review.Id, ProductId = review.ProductId, UserId = review.UserId,
                Rating = review.Rating, Comment = review.Comment, CreatedAt = review.CreatedAt
            }).ToListAsync();

    public async Task<bool> ApproveAsync(int actorId, int reviewId)
    {
        var review = await context.Reviews.FindAsync(reviewId);
        if (review is null) return false;
        review.IsApproved = true;
        review.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        if (audit is not null) await audit.RecordAsync(actorId, "Approve", "Review", reviewId, "Review approved", null);
        return true;
    }

    public async Task<bool> DeleteAsync(int actorId, int reviewId)
    {
        var review = await context.Reviews.FindAsync(reviewId);
        if (review is null) return false;
        context.Reviews.Remove(review);
        await context.SaveChangesAsync();
        if (audit is not null) await audit.RecordAsync(actorId, "Delete", "Review", reviewId, "Review deleted", null);
        return true;
    }

    public async Task<IReadOnlyList<ReviewResponse>> GetApprovedAsync(int productId) =>
        await context.Reviews.AsNoTracking()
            .Where(review => review.ProductId == productId && review.IsApproved)
            .OrderByDescending(review => review.CreatedAt)
            .Select(review => new ReviewResponse
            {
                Id = review.Id, ProductId = review.ProductId, UserId = review.UserId,
                Rating = review.Rating, Comment = review.Comment, CreatedAt = review.CreatedAt
            })
            .ToListAsync();

    public async Task<ReviewResponse> CreateAsync(int userId, int productId, ReviewRequest request)
    {
        var productExists = await context.Products.AnyAsync(product => product.Id == productId && product.IsActive);
        if (!productExists) throw new KeyNotFoundException("Product not found.");
        var purchased = await context.OrderItems.AnyAsync(item => item.ProductId == productId &&
            context.Orders.Any(order => order.Id == item.OrderId && order.UserId == userId && order.Status != "Cancelled"));
        if (!purchased) throw new InvalidOperationException("You can review only products you purchased.");
        if (await context.Reviews.AnyAsync(review => review.ProductId == productId && review.UserId == userId))
            throw new InvalidOperationException("You already reviewed this product.");

        var review = new Review { ProductId = productId, UserId = userId, Rating = request.Rating, Comment = request.Comment?.Trim() };
        context.Reviews.Add(review);
        await context.SaveChangesAsync();
        return ToResponse(review);
    }

    private static ReviewResponse ToResponse(Review review) => new()
    {
        Id = review.Id, ProductId = review.ProductId, UserId = review.UserId,
        Rating = review.Rating, Comment = review.Comment, CreatedAt = review.CreatedAt
    };
}
