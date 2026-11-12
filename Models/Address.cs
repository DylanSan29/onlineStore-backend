namespace OnlineStoreBackend.Models;

public sealed class Address
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public required string FullName { get; set; }
    public required string AddressLine { get; set; }
    public required string City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public required string Country { get; set; }
    public bool IsDefault { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
