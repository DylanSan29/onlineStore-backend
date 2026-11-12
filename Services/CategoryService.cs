using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class CategoryService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<CategoryResponse>> GetAllAsync()
    {
        var categories = await context.Categories.AsNoTracking()
            .Where(category => category.IsActive).OrderBy(category => category.Name).ToListAsync();
        return categories.Select(ToResponse).ToList();
    }

    public async Task<CategoryResponse?> GetByIdAsync(int id)
    {
        var category = await context.Categories.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id && item.IsActive);
        return category is null ? null : ToResponse(category);
    }

    public async Task<CategoryResponse> CreateAsync(CategoryRequest request)
    {
        var name = request.Name.Trim();
        if (await context.Categories.AnyAsync(category => category.Name == name && category.IsActive))
            throw new InvalidOperationException("Category name is already in use");

        var category = new Category { Name = name, Description = request.Description?.Trim() };
        context.Categories.Add(category);
        await context.SaveChangesAsync();
        return ToResponse(category);
    }

    public async Task<bool> UpdateAsync(int id, CategoryRequest request)
    {
        var category = await context.Categories.FirstOrDefaultAsync(item => item.Id == id && item.IsActive);
        if (category is null) return false;

        var name = request.Name.Trim();
        if (await context.Categories.AnyAsync(item => item.Id != id && item.Name == name && item.IsActive))
            throw new InvalidOperationException("Category name is already in use");

        category.Name = name;
        category.Description = request.Description?.Trim();
        category.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var category = await context.Categories.FirstOrDefaultAsync(item => item.Id == id && item.IsActive);
        if (category is null) return false;
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    private static CategoryResponse ToResponse(Category category) => new()
    {
        Id = category.Id, Name = category.Name, Description = category.Description, IsActive = category.IsActive
    };
}
