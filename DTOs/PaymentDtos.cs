namespace OnlineStoreBackend.DTOs;

public sealed class PaymentCheckoutResponse
{
    public long PaymentId { get; init; }
    public required string Provider { get; init; }
    public required string Status { get; init; }
    public decimal Amount { get; init; }
    public required string Currency { get; init; }
    public string? CheckoutUrl { get; init; }
    public string? Message { get; init; }
}

public sealed class PaymentWebhookRequest
{
    public long PaymentId { get; set; }
    public string? ProviderPaymentId { get; set; }
    public int OrderId { get; set; }
    public string? Type { get; set; }
    public string? Status { get; set; }
}
