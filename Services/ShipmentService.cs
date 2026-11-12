using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class ShipmentService(OnlineStoreContext context)
{
    private static readonly string[] AllowedStatuses = ["Pending", "Packed", "Shipped", "InTransit", "OutForDelivery", "Delivered", "Returned"];

    public async Task<ShipmentResponse?> GetForOrderAsync(int userId, bool isAdmin, int orderId)
    {
        var ownsOrder = await context.Orders.AnyAsync(order => order.Id == orderId && (isAdmin || order.UserId == userId));
        if (!ownsOrder) return null;
        var shipment = await context.Shipments.AsNoTracking().SingleOrDefaultAsync(item => item.OrderId == orderId);
        return shipment is null ? null : ToResponse(shipment);
    }

    public async Task<ShipmentResponse?> UpdateAsync(int orderId, ShipmentUpdateRequest request)
    {
        var orderExists = await context.Orders.AnyAsync(order => order.Id == orderId);
        if (!orderExists) return null;
        if (!AllowedStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid shipment status.");

        var shipment = await context.Shipments.SingleOrDefaultAsync(item => item.OrderId == orderId);
        if (shipment is null)
        {
            shipment = new Shipment { OrderId = orderId, Status = request.Status };
            context.Shipments.Add(shipment);
        }
        shipment.Carrier = request.Carrier?.Trim();
        shipment.TrackingNumber = request.TrackingNumber?.Trim();
        shipment.Status = request.Status;
        shipment.UpdatedAt = DateTime.UtcNow;
        if (request.Status.Equals("Shipped", StringComparison.OrdinalIgnoreCase) && shipment.ShippedAt is null)
            shipment.ShippedAt = DateTime.UtcNow;
        if (request.Status.Equals("Delivered", StringComparison.OrdinalIgnoreCase) && shipment.DeliveredAt is null)
            shipment.DeliveredAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return ToResponse(shipment);
    }

    private static ShipmentResponse ToResponse(Shipment shipment) => new()
    {
        Id = shipment.Id, OrderId = shipment.OrderId, Carrier = shipment.Carrier,
        TrackingNumber = shipment.TrackingNumber, Status = shipment.Status,
        ShippedAt = shipment.ShippedAt, DeliveredAt = shipment.DeliveredAt, UpdatedAt = shipment.UpdatedAt
    };
}
