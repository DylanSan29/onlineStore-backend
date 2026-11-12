namespace OnlineStoreBackend.DTOs;

public sealed class PaymentCheckoutResponse
{
    public required string Provider { get; init; }
    public required string Status { get; init; }
    public string? CheckoutUrl { get; init; }
    public string? Message { get; init; }
}

public sealed class PaymentWebhookRequest
{
    public string? Type { get; set; }
    public string? DataId { get; set; }
}
