namespace OnlineStoreBackend.Models;

public sealed class Wishlist
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public List<WishlistItem> Items { get; set; } = [];
}
