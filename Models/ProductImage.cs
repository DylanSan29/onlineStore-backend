namespace OnlineStoreBackend.Models;

public sealed class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public required string ImageUrl { get; set; }
    public string? AltText { get; set; }
    public bool IsPrimary { get; set; }
    public int SortOrder { get; set; }
}
