namespace OnlineStoreBackend.DTOs;

public sealed class NotificationResponse
{
    public int Id { get; init; }
    public required string Type { get; init; }
    public required string Title { get; init; }
    public required string Message { get; init; }
    public bool IsRead { get; init; }
    public DateTime? ReadAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

public sealed class NotificationCountResponse
{
    public int UnreadCount { get; init; }
}
