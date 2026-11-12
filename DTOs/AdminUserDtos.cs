using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class UserAdminResponse
{
    public int Id { get; init; }
    public required string Username { get; init; }
    public required string Role { get; init; }
    public bool IsActive { get; init; }
}

public sealed class UpdateUserRoleRequest
{
    [Required, StringLength(30)] public required string Role { get; set; }
}
