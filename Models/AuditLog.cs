namespace OnlineStoreBackend.Models;

public sealed class AuditLog
{
    public long Id { get; set; }
    public int? UserId { get; set; }
    public required string Action { get; set; }
    public required string EntityName { get; set; }
    public int? EntityId { get; set; }
    public string? Details { get; set; }
    public string? IpAddress { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
