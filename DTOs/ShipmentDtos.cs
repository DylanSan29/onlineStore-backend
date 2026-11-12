using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class ShipmentUpdateRequest
{
    [StringLength(80)] public string? Carrier { get; set; }
    [StringLength(120)] public string? TrackingNumber { get; set; }
    [Required, StringLength(30)] public required string Status { get; set; }
}

public sealed class ShipmentResponse
{
    public int Id { get; init; }
    public int OrderId { get; init; }
    public string? Carrier { get; init; }
    public string? TrackingNumber { get; init; }
    public required string Status { get; init; }
    public DateTime? ShippedAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
