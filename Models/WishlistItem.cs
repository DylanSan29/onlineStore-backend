namespace OnlineStoreBackend.Models;

public sealed class WishlistItem
{
    public int Id { get; set; }
    public int WishlistId { get; set; }
    public int ProductId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
