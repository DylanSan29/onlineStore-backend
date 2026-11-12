using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class OrderService(OnlineStoreContext context)
{
    private static readonly string[] AllowedStatuses = ["Pending", "Paid", "Processing", "Shipped", "Delivered", "Cancelled", "Refunded"];

    public async Task<OrderResponse> CreateAsync(int userId, CreateOrderRequest request)
    {
        var requestedItems = request.Items.GroupBy(item => item.ProductId)
            .Select(group => new RequestedItem(group.Key, group.Sum(item => item.Quantity)))
            .ToList();
        var products = await context.Products.Include(product => product.Inventory)
            .Where(product => requestedItems.Select(item => item.ProductId).Contains(product.Id) && product.IsActive)
            .ToDictionaryAsync(product => product.Id);

        var items = BuildItems(requestedItems, products);
        var subtotal = items.Sum(item => item.LineTotal);
        var order = new Order
        {
            UserId = userId, Status = "Pending", Subtotal = subtotal, ShippingCost = 0,
            Tax = 0, Total = subtotal, ShippingFullName = request.ShippingAddress.FullName.Trim(),
            ShippingAddress = request.ShippingAddress.Address.Trim(), ShippingCity = request.ShippingAddress.City.Trim(),
            ShippingPostalCode = request.ShippingAddress.PostalCode?.Trim(), Items = items
        };

        foreach (var item in requestedItems)
        {
            var product = products[item.ProductId];
            if (product.Inventory is not null)
                product.Inventory.QuantityOnHand -= item.Quantity;
            product.Quantity -= item.Quantity;
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync();
        return ToResponse(order);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetForUserAsync(int userId)
    {
        var orders = await context.Orders.AsNoTracking().Include(order => order.Items)
            .Where(order => order.UserId == userId).OrderByDescending(order => order.CreatedAt).ToListAsync();
        return orders.Select(ToResponse).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(int userId, bool isAdmin, int orderId)
    {
        var order = await context.Orders.AsNoTracking().Include(item => item.Items)
            .FirstOrDefaultAsync(item => item.Id == orderId && (isAdmin || item.UserId == userId));
        return order is null ? null : ToResponse(order);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetAllAsync()
    {
        var orders = await context.Orders.AsNoTracking().Include(order => order.Items)
            .OrderByDescending(order => order.CreatedAt).ToListAsync();
        return orders.Select(ToResponse).ToList();
    }

    public async Task<bool> UpdateStatusAsync(int orderId, string status)
    {
        if (!AllowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException("Invalid order status");

        var order = await context.Orders.FirstOrDefaultAsync(item => item.Id == orderId);
        if (order is null) return false;
        order.Status = status;
        order.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> CancelAsync(int userId, bool isAdmin, int orderId)
    {
        var order = await context.Orders.FirstOrDefaultAsync(item => item.Id == orderId && (isAdmin || item.UserId == userId));
        if (order is null) return false;
        if (order.Status is "Shipped" or "Delivered" or "Cancelled") return false;
        order.Status = "Cancelled";
        order.UpdatedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    private static List<OrderItem> BuildItems(IEnumerable<RequestedItem> requestedItems, IReadOnlyDictionary<int, Product> products)
    {
        var items = new List<OrderItem>();
        foreach (var requested in requestedItems)
        {
            if (!products.TryGetValue(requested.ProductId, out var product))
                throw new InvalidOperationException($"Product {requested.ProductId} was not found");
            var availableQuantity = product.Inventory?.QuantityOnHand ?? product.Quantity;
            if (availableQuantity < requested.Quantity)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}");
            items.Add(new OrderItem
            {
                ProductId = product.Id, ProductName = product.Name, UnitPrice = product.Price,
                Quantity = requested.Quantity, LineTotal = product.Price * requested.Quantity
            });
        }
        return items;
    }

    private static OrderResponse ToResponse(Order order) => new()
    {
        Id = order.Id, UserId = order.UserId, Status = order.Status, Subtotal = order.Subtotal,
        ShippingCost = order.ShippingCost, Tax = order.Tax, Total = order.Total, CreatedAt = order.CreatedAt,
        ShippingAddress = new ShippingAddressResponse
        {
            FullName = order.ShippingFullName, Address = order.ShippingAddress,
            City = order.ShippingCity, PostalCode = order.ShippingPostalCode
        },
        Items = order.Items.Select(item => new OrderItemResponse
        {
            ProductId = item.ProductId, ProductName = item.ProductName, UnitPrice = item.UnitPrice,
            Quantity = item.Quantity, LineTotal = item.LineTotal
        }).ToList()
    };

    private sealed record RequestedItem(int ProductId, int Quantity);
}
