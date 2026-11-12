namespace OnlineStoreBackend.DTOs;

public sealed class WishlistItemResponse
{
    public int ProductId { get; init; }
    public required string Name { get; init; }
    public decimal Price { get; init; }
    public string? ImageUrl { get; init; }
    public DateTime AddedAt { get; init; }
}
