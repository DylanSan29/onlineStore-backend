using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class ProductImageRequest
{
    [Required, Url, StringLength(1000)] public required string ImageUrl { get; set; }
    [StringLength(160)] public string? AltText { get; set; }
    [Range(0, int.MaxValue)] public int SortOrder { get; set; }
    public bool IsPrimary { get; set; }
}

public sealed class ProductImageResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public required string ImageUrl { get; init; }
    public string? AltText { get; init; }
    public bool IsPrimary { get; init; }
    public int SortOrder { get; init; }
}
