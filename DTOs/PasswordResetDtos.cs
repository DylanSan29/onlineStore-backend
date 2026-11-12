using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class PasswordResetRequest
{
    [Required, StringLength(100)] public required string Username { get; set; }
}

public sealed class PasswordResetConfirmRequest
{
    [Required] public required string Token { get; set; }
    [Required, MinLength(8), StringLength(128)] public required string NewPassword { get; set; }
}

public sealed class PasswordResetResult
{
    public bool UserFound { get; init; }
    public string? Token { get; init; }
}
