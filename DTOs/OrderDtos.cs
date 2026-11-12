using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class CreateOrderRequest
{
    [Required, MinLength(1)] public required List<CreateOrderItemRequest> Items { get; set; }
    [Required] public required ShippingAddressRequest ShippingAddress { get; set; }
    [StringLength(40)] public string? CouponCode { get; set; }
}

public sealed class CreateOrderItemRequest
{
    [Range(1, int.MaxValue)] public int ProductId { get; set; }
    [Range(1, 1000)] public int Quantity { get; set; }
}

public sealed class ShippingAddressRequest
{
    [Required, StringLength(160)] public required string FullName { get; set; }
    [Required, StringLength(300)] public required string Address { get; set; }
    [Required, StringLength(100)] public required string City { get; set; }
    [StringLength(30)] public string? PostalCode { get; set; }
}

public sealed class UpdateOrderStatusRequest
{
    [Required] public required string Status { get; set; }
}

public sealed class OrderResponse
{
    public int Id { get; init; }
    public int UserId { get; init; }
    public required string Status { get; init; }
    public decimal Subtotal { get; init; }
    public decimal ShippingCost { get; init; }
    public decimal Tax { get; init; }
    public decimal Discount { get; init; }
    public decimal Total { get; init; }
    public required ShippingAddressResponse ShippingAddress { get; init; }
    public DateTime CreatedAt { get; init; }
    public required List<OrderItemResponse> Items { get; init; }
}

public sealed class ShippingAddressResponse
{
    public required string FullName { get; init; }
    public required string Address { get; init; }
    public required string City { get; init; }
    public string? PostalCode { get; init; }
}

public sealed class OrderItemResponse
{
    public int ProductId { get; init; }
    public required string ProductName { get; init; }
    public decimal UnitPrice { get; init; }
    public int Quantity { get; init; }
    public decimal LineTotal { get; init; }
}
