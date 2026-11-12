namespace OnlineStoreBackend.DTOs;

public sealed class AuditLogQuery
{
    public string? Action { get; set; }
    public string? EntityName { get; set; }
    public int? UserId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 50;
}

public sealed class AuditLogResponse
{
    public long Id { get; init; }
    public int? UserId { get; init; }
    public required string Action { get; init; }
    public required string EntityName { get; init; }
    public int? EntityId { get; init; }
    public string? Details { get; init; }
    public string? IpAddress { get; init; }
    public DateTime CreatedAt { get; init; }
}
