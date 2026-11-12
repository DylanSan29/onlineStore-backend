using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class CategoryRequest
{
    [Required, StringLength(80)] public required string Name { get; set; }
    [StringLength(500)] public string? Description { get; set; }
}

public sealed class CategoryResponse
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public bool IsActive { get; init; }
}
