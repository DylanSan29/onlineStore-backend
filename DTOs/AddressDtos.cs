using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class AddressRequest
{
    [Required, StringLength(160)] public required string FullName { get; set; }
    [Required, StringLength(300)] public required string AddressLine { get; set; }
    [Required, StringLength(100)] public required string City { get; set; }
    [StringLength(100)] public string? State { get; set; }
    [StringLength(30)] public string? PostalCode { get; set; }
    [Required, StringLength(80)] public required string Country { get; set; }
    public bool IsDefault { get; set; }
}

public sealed class AddressResponse
{
    public int Id { get; init; }
    public required string FullName { get; init; }
    public required string AddressLine { get; init; }
    public required string City { get; init; }
    public string? State { get; init; }
    public string? PostalCode { get; init; }
    public required string Country { get; init; }
    public bool IsDefault { get; init; }
}
