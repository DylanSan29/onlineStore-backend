namespace OnlineStoreBackend.Models;

public sealed class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string Status { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal Tax { get; set; }
    public decimal Total { get; set; }
    public required string ShippingFullName { get; set; }
    public required string ShippingAddress { get; set; }
    public required string ShippingCity { get; set; }
    public string? ShippingPostalCode { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = [];
}
