using System.ComponentModel.DataAnnotations;

namespace OnlineStoreBackend.DTOs;

public sealed class EmailVerificationRequest
{
    [Required, StringLength(100)] public required string Username { get; set; }
}

public sealed class EmailVerificationConfirmRequest
{
    [Required] public required string Token { get; set; }
}
