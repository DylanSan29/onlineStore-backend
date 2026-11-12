using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class PaymentService(OnlineStoreContext context, IConfiguration configuration, NotificationService? notifications = null)
{
    public async Task<PaymentCheckoutResponse?> CreateCheckoutAsync(int userId, int orderId, string? idempotencyKey)
    {
        if (string.IsNullOrWhiteSpace(idempotencyKey) || idempotencyKey.Length > 100)
            throw new InvalidOperationException("A valid idempotency key is required.");
        var order = await context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == orderId && item.UserId == userId);
        if (order is null) return null;
        if (order.Status is "Cancelled" or "Refunded")
            throw new InvalidOperationException("This order cannot be paid.");

        var existing = await context.Payments.SingleOrDefaultAsync(payment => payment.IdempotencyKey == idempotencyKey);
        if (existing is not null) return ToResponse(existing, "Checkout already exists for this idempotency key.");
        var provider = configuration["Payments:MercadoPago:Provider"] ?? "MercadoPago";
        var enabled = configuration.GetValue("Payments:MercadoPago:Enabled", false);
        var status = enabled && !string.IsNullOrWhiteSpace(configuration["Payments:MercadoPago:AccessToken"])
            ? "PendingProvider" : "NotConfigured";
        var payment = new Payment
        {
            OrderId = orderId, UserId = userId, Provider = provider, Status = status,
            Amount = order.Total, Currency = configuration["Payments:Currency"] ?? "MXN", IdempotencyKey = idempotencyKey
        };
        context.Payments.Add(payment);
        await context.SaveChangesAsync();
        return ToResponse(payment, status == "NotConfigured" ? "Provider credentials are not configured yet." : "Provider adapter is ready for API integration.");
    }

    public async Task<PaymentCheckoutResponse?> GetStatusAsync(int userId, int orderId)
    {
        var payment = await context.Payments.AsNoTracking().Where(item => item.OrderId == orderId && item.UserId == userId)
            .OrderByDescending(item => item.CreatedAt).FirstOrDefaultAsync();
        return payment is null ? null : ToResponse(payment, null);
    }

    public async Task<bool> ProcessWebhookAsync(PaymentWebhookRequest request, string? signature)
    {
        var secret = configuration["Payments:MercadoPago:WebhookSecret"];
        if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(signature)) return false;
        var expected = Encoding.UTF8.GetBytes(secret);
        var provided = Encoding.UTF8.GetBytes(signature);
        if (!CryptographicOperations.FixedTimeEquals(expected, provided)) return false;
        var payment = await context.Payments.FirstOrDefaultAsync(item => item.Id == request.PaymentId);
        if (payment is null) return false;
        payment.ProviderPaymentId = request.ProviderPaymentId;
        payment.Status = request.Status?.ToLowerInvariant() switch
        {
            "approved" or "paid" => "Paid", "rejected" or "cancelled" or "failed" => "Failed", _ => "Pending"
        };
        payment.UpdatedAt = DateTime.UtcNow;
        if (payment.Status == "Paid")
        {
            var order = await context.Orders.FindAsync(payment.OrderId);
            if (order is not null) { order.Status = "Paid"; order.UpdatedAt = DateTime.UtcNow; }
        }
        await context.SaveChangesAsync();
        if (notifications is not null)
            await notifications.CreateAsync(payment.UserId, new CreateNotificationRequest
            {
                Type = "Payment", Title = "Payment status updated", Message = $"Payment #{payment.Id} status: {payment.Status}."
            });
        return true;
    }

    private static PaymentCheckoutResponse ToResponse(Payment payment, string? message) => new()
    {
        PaymentId = payment.Id, Provider = payment.Provider, Status = payment.Status,
        Amount = payment.Amount, Currency = payment.Currency, Message = message
    };
}
