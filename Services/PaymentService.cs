using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;

namespace OnlineStoreBackend.Services;

public sealed class PaymentService(OnlineStoreContext context, IConfiguration configuration)
{
    public async Task<PaymentCheckoutResponse?> CreateCheckoutAsync(int userId, int orderId)
    {
        var order = await context.Orders.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == orderId && item.UserId == userId);
        if (order is null) return null;
        if (order.Status is "Cancelled" or "Refunded")
            throw new InvalidOperationException("This order cannot be paid.");

        var provider = configuration["Payments:MercadoPago:Provider"] ?? "MercadoPago";
        var enabled = configuration.GetValue("Payments:MercadoPago:Enabled", false);
        if (!enabled)
            return new PaymentCheckoutResponse
            {
                Provider = provider, Status = "NotConfigured",
                Message = "Payment provider credentials are not configured yet."
            };

        if (string.IsNullOrWhiteSpace(configuration["Payments:MercadoPago:AccessToken"]))
            throw new InvalidOperationException("Payment provider is enabled but credentials are missing.");

        return new PaymentCheckoutResponse
        {
            Provider = provider, Status = "ProviderIntegrationPending",
            Message = "The provider adapter is ready for Mercado Pago SDK/API integration."
        };
    }

    public async Task<PaymentCheckoutResponse?> GetStatusAsync(int userId, int orderId)
    {
        var exists = await context.Orders.AnyAsync(order => order.Id == orderId && order.UserId == userId);
        if (!exists) return null;
        return new PaymentCheckoutResponse
        {
            Provider = configuration["Payments:MercadoPago:Provider"] ?? "MercadoPago",
            Status = "NotConfigured",
            Message = "Payment status will be available after provider integration."
        };
    }
}
