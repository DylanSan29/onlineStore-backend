namespace OnlineStoreBackend.Models;

public sealed class Payment
{
    public long Id { get; set; }
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public required string Provider { get; set; }
    public string? ProviderPaymentId { get; set; }
    public required string Status { get; set; }
    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public required string IdempotencyKey { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
