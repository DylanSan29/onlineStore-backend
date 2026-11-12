using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class InventoryAdjustmentRequest
{
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(-1_000_000, 1_000_000)] public int QuantityChanged { get; set; }
    [Required, StringLength(40)] public required string MovementType { get; set; }
    [StringLength(500)] public string? Reason { get; set; }
}

public sealed class InventoryItemResponse
{
    public int ProductId { get; init; }
    public required string ProductName { get; init; }
    public int Quantity { get; init; }
    public bool LowStock { get; init; }
}

public sealed class InventoryMovementResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int QuantityChanged { get; init; }
    public required string MovementType { get; init; }
    public string? Reason { get; init; }
    public int CreatedByUserId { get; init; }
    public DateTime CreatedAt { get; init; }
}
