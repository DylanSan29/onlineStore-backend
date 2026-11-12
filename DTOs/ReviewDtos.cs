using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class ReviewRequest
{
    [Range(1, 5)] public int Rating { get; set; }
    [StringLength(1000)] public string? Comment { get; set; }
}

public sealed class ReviewResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int UserId { get; init; }
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public DateTime CreatedAt { get; init; }
}
