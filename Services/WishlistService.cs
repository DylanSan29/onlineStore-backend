using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class WishlistService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<WishlistItemResponse>> GetAsync(int userId)
    {
        return await context.WishlistItems
            .Where(item => context.Wishlists.Any(list => list.Id == item.WishlistId && list.UserId == userId))
            .Join(context.Products, item => item.ProductId, product => product.Id, (item, product) => new WishlistItemResponse
            {
                ProductId = product.Id, Name = product.Name, Price = product.Price,
                ImageUrl = product.ImageUrl, AddedAt = item.CreatedAt
            })
            .OrderByDescending(item => item.AddedAt)
            .ToListAsync();
    }

    public async Task<WishlistItemResponse?> AddAsync(int userId, int productId)
    {
        var product = await context.Products.FirstOrDefaultAsync(item => item.Id == productId && item.IsActive);
        if (product is null) return null;

        var wishlist = await context.Wishlists.SingleOrDefaultAsync(item => item.UserId == userId);
        if (wishlist is null)
        {
            wishlist = new Wishlist { UserId = userId };
            context.Wishlists.Add(wishlist);
            await context.SaveChangesAsync();
        }

        var alreadyAdded = await context.WishlistItems.AnyAsync(item => item.WishlistId == wishlist.Id && item.ProductId == productId);
        if (alreadyAdded) return ToResponse(product, DateTime.UtcNow);

        var wishlistItem = new WishlistItem { WishlistId = wishlist.Id, ProductId = productId };
        context.WishlistItems.Add(wishlistItem);
        wishlist.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return ToResponse(product, wishlistItem.CreatedAt);
    }

    public async Task<bool> RemoveAsync(int userId, int productId)
    {
        var item = await context.WishlistItems.FirstOrDefaultAsync(wishlistItem =>
            wishlistItem.ProductId == productId && context.Wishlists.Any(list => list.Id == wishlistItem.WishlistId && list.UserId == userId));
        if (item is null) return false;
        context.WishlistItems.Remove(item);
        await context.SaveChangesAsync();
        return true;
    }

    private static WishlistItemResponse ToResponse(Product product, DateTime addedAt) => new()
    {
        ProductId = product.Id, Name = product.Name, Price = product.Price,
        ImageUrl = product.ImageUrl, AddedAt = addedAt
    };
}
