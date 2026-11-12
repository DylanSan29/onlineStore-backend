using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class ProductQuery
{
    public string? Search { get; set; }
    public string? Category { get; set; }
    public bool InStock { get; set; }
    [Range(0, 1_000_000)] public decimal? MinPrice { get; set; }
    [Range(0, 1_000_000)] public decimal? MaxPrice { get; set; }
}

public class ProductCreateRequest
{
    [Required, StringLength(160)] public required string Name { get; set; }
    [Required, StringLength(2000)] public required string Description { get; set; }
    [Range(0, 1_000_000)] public decimal Price { get; set; }
    [Range(0, int.MaxValue)] public int Quantity { get; set; }
    [StringLength(80)] public string? SKU { get; set; }
    [StringLength(80)] public string? Category { get; set; }
    [Url, StringLength(500)] public string? ImageUrl { get; set; }
    [Range(0, 1_000_000)] public decimal? Discount { get; set; }
    [StringLength(80)] public string? Brand { get; set; }
}

public sealed class ProductUpdateRequest : ProductCreateRequest
{
}

public sealed class ProductResponse
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public string? SKU { get; init; }
    public string? Category { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? Discount { get; init; }
    public string? Brand { get; init; }
    public bool IsActive { get; init; }
}
