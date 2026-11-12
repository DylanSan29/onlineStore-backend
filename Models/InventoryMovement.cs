namespace OnlineStoreBackend.Models;

public sealed class InventoryMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int QuantityChanged { get; set; }
    public required string MovementType { get; set; }
    public string? Reason { get; set; }
    public int CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
