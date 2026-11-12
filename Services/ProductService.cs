using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class ProductService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<ProductResponse>> GetAllAsync(ProductQuery query)
    {
        var products = context.Products.AsNoTracking().Include(product => product.Inventory).Where(product => product.IsActive);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            products = products.Where(product => product.Name.ToLower().Contains(search) || product.Description.ToLower().Contains(search));
        }
        if (!string.IsNullOrWhiteSpace(query.Category))
            products = products.Where(product => product.Category == query.Category);
        if (query.InStock)
            products = products.Where(product => (product.Inventory != null ? product.Inventory.QuantityOnHand : product.Quantity) > 0);
        if (query.MinPrice.HasValue)
            products = products.Where(product => product.Price >= query.MinPrice.Value);
        if (query.MaxPrice.HasValue)
            products = products.Where(product => product.Price <= query.MaxPrice.Value);

        var result = await products.OrderBy(product => product.Name).ToListAsync();
        return result.Select(ToResponse).ToList();
    }

    public async Task<ProductResponse?> GetByIdAsync(int id)
    {
        var product = await context.Products.AsNoTracking().Include(item => item.Inventory)
            .Where(product => product.Id == id && product.IsActive)
            .FirstOrDefaultAsync();
        return product is null ? null : ToResponse(product);
    }

    public async Task<ProductResponse> CreateAsync(ProductCreateRequest request)
    {
        var product = new Product
        {
            Name = request.Name.Trim(), Description = request.Description.Trim(), Price = request.Price,
            Quantity = request.Quantity, SKU = request.SKU, Category = request.Category,
            Inventory = new Inventory { QuantityOnHand = request.Quantity },
            ImageUrl = request.ImageUrl, Discount = request.Discount, Brand = request.Brand
        };

        context.Products.Add(product);
        await context.SaveChangesAsync();
        return ToResponse(product);
    }

    public async Task<bool> UpdateAsync(int id, ProductUpdateRequest request)
    {
        var product = await context.Products.FirstOrDefaultAsync(item => item.Id == id && item.IsActive);
        if (product is null) return false;

        product.Name = request.Name.Trim();
        product.Description = request.Description.Trim();
        product.Price = request.Price;
        product.Quantity = request.Quantity;
        if (product.Inventory is null)
            product.Inventory = new Inventory { QuantityOnHand = request.Quantity };
        else
            product.Inventory.QuantityOnHand = request.Quantity;
        product.SKU = request.SKU;
        product.Category = request.Category;
        product.ImageUrl = request.ImageUrl;
        product.Discount = request.Discount;
        product.Brand = request.Brand;
        product.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var product = await context.Products.FirstOrDefaultAsync(item => item.Id == id && item.IsActive);
        if (product is null) return false;

        product.IsActive = false;
        product.DeletedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    private static ProductResponse ToResponse(Product product) => new()
    {
        Id = product.Id, Name = product.Name, Description = product.Description,
        Price = product.Price, Quantity = product.Inventory?.QuantityOnHand ?? product.Quantity, SKU = product.SKU,
        Category = product.Category, ImageUrl = product.ImageUrl, Discount = product.Discount,
        Brand = product.Brand, IsActive = product.IsActive
    };
}
