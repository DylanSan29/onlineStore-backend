using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class InventoryService(OnlineStoreContext context, AuditLogService? audit = null)
{
    public async Task<IReadOnlyList<InventoryItemResponse>> GetAllAsync(int threshold)
    {
        var inventory = await context.Inventories.AsNoTracking().Include(item => item.Product)
            .Where(item => item.Product != null && item.Product.IsActive)
            .OrderBy(item => item.Product!.Name).ToListAsync();
        return inventory.Select(item => ToItem(item, threshold)).ToList();
    }

    public async Task<IReadOnlyList<InventoryItemResponse>> GetLowStockAsync(int threshold)
    {
        var inventory = await context.Inventories.AsNoTracking().Include(item => item.Product)
            .Where(item => item.Product != null && item.Product.IsActive && item.QuantityOnHand <= threshold)
            .OrderBy(item => item.QuantityOnHand).ToListAsync();
        return inventory.Select(item => ToItem(item, threshold)).ToList();
    }

    public async Task<IReadOnlyList<InventoryMovementResponse>> GetMovementsAsync()
    {
        var movements = await context.InventoryMovements.AsNoTracking()
            .OrderByDescending(movement => movement.CreatedAt).ToListAsync();
        return movements.Select(ToMovement).ToList();
    }

    public async Task<InventoryItemResponse> AdjustAsync(int userId, InventoryAdjustmentRequest request)
    {
        var inventory = await context.Inventories.Include(item => item.Product)
            .FirstOrDefaultAsync(item => item.ProductId == request.ProductId && item.Product!.IsActive);
        if (inventory?.Product is null) throw new KeyNotFoundException("Product not found");

        var newQuantity = inventory.QuantityOnHand + request.QuantityChanged;
        if (newQuantity < 0) throw new InvalidOperationException("Inventory cannot be negative");

        inventory.QuantityOnHand = newQuantity;
        inventory.Product.Quantity = newQuantity;
        inventory.Product.UpdatedAt = DateTime.UtcNow;
        context.InventoryMovements.Add(new InventoryMovement
        {
            ProductId = inventory.ProductId, QuantityChanged = request.QuantityChanged,
            MovementType = request.MovementType.Trim(), Reason = request.Reason?.Trim(), CreatedByUserId = userId
        });
        await context.SaveChangesAsync();
        if (audit is not null) await audit.RecordAsync(userId, "Adjust", "Inventory", request.ProductId, $"Quantity changed by {request.QuantityChanged}", null);
        return ToItem(inventory, inventory.ReorderLevel);
    }

    private static InventoryItemResponse ToItem(Inventory inventory, int threshold) => new()
    {
        ProductId = inventory.ProductId, ProductName = inventory.Product!.Name,
        Quantity = inventory.QuantityOnHand, LowStock = inventory.QuantityOnHand <= threshold
    };

    private static InventoryMovementResponse ToMovement(InventoryMovement movement) => new()
    {
        Id = movement.Id, ProductId = movement.ProductId, QuantityChanged = movement.QuantityChanged,
        MovementType = movement.MovementType, Reason = movement.Reason,
        CreatedByUserId = movement.CreatedByUserId, CreatedAt = movement.CreatedAt
    };
}
