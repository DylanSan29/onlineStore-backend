namespace OnlineStoreBackend.Models;

public sealed class Inventory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int QuantityOnHand { get; set; }
    public int QuantityReserved { get; set; }
    public int ReorderLevel { get; set; } = 5;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Product? Product { get; set; }
}
