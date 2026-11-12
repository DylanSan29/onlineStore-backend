namespace OnlineStoreBackend.DTOs;

public sealed class HealthResponse
{
    public required string Status { get; init; }
    public required string Service { get; init; }
    public DateTime CheckedAt { get; init; }
}
