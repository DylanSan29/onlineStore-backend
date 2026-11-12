using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Route("api/auth/email-verification")]
public sealed class EmailVerificationController(EmailVerificationService verification) : ControllerBase
{
    [HttpPost("request")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> RequestVerification(EmailVerificationRequest request)
    {
        await verification.RequestAsync(request.Username);
        return Ok(new { message = "If the account exists, verification instructions will be sent." });
    }

    [HttpPost("confirm")]
    [AllowAnonymous]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Confirm(EmailVerificationConfirmRequest request) =>
        await verification.ConfirmAsync(request.Token) ? NoContent() : BadRequest(new { message = "Invalid or expired verification token." });
}
