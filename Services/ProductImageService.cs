using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class ProductImageService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<ProductImageResponse>> GetAsync(int productId) =>
        await context.ProductImages.AsNoTracking()
            .Where(image => image.ProductId == productId)
            .OrderByDescending(image => image.IsPrimary).ThenBy(image => image.SortOrder)
            .Select(image => new ProductImageResponse
            {
                Id = image.Id, ProductId = image.ProductId, ImageUrl = image.ImageUrl,
                AltText = image.AltText, IsPrimary = image.IsPrimary, SortOrder = image.SortOrder
            }).ToListAsync();

    public async Task<ProductImageResponse?> AddAsync(int productId, ProductImageRequest request)
    {
        var product = await context.Products.FirstOrDefaultAsync(item => item.Id == productId && item.IsActive);
        if (product is null) return null;

        if (request.IsPrimary)
            await ClearPrimaryAsync(productId);

        var image = new ProductImage
        {
            ProductId = productId, ImageUrl = request.ImageUrl.Trim(),
            AltText = request.AltText?.Trim(), IsPrimary = request.IsPrimary, SortOrder = request.SortOrder
        };
        context.ProductImages.Add(image);
        if (image.IsPrimary) product.ImageUrl = image.ImageUrl;
        await context.SaveChangesAsync();
        return ToResponse(image);
    }

    public async Task<bool> DeleteAsync(int productId, int imageId)
    {
        var image = await context.ProductImages.FirstOrDefaultAsync(item => item.Id == imageId && item.ProductId == productId);
        if (image is null) return false;
        context.ProductImages.Remove(image);
        var product = await context.Products.FindAsync(productId);
        if (product?.ImageUrl == image.ImageUrl)
        {
            var replacement = await context.ProductImages.Where(item => item.ProductId == productId && item.Id != imageId)
                .OrderBy(item => item.SortOrder).FirstOrDefaultAsync();
            product.ImageUrl = replacement?.ImageUrl;
            if (replacement != null) replacement.IsPrimary = true;
        }
        await context.SaveChangesAsync();
        return true;
    }

    private async Task ClearPrimaryAsync(int productId)
    {
        var primaryImages = await context.ProductImages.Where(image => image.ProductId == productId && image.IsPrimary).ToListAsync();
        foreach (var image in primaryImages)
            image.IsPrimary = false;
    }

    private static ProductImageResponse ToResponse(ProductImage image) => new()
    {
        Id = image.Id, ProductId = image.ProductId, ImageUrl = image.ImageUrl,
        AltText = image.AltText, IsPrimary = image.IsPrimary, SortOrder = image.SortOrder
    };
}
