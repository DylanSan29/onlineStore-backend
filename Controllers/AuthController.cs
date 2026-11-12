using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;
using Microsoft.AspNetCore.RateLimiting;

namespace OnlineStoreBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var result = await _authService.Login(loginDto);
                return Ok(new
                {
                    token = result.Token,
                    refreshToken = result.RefreshToken,
                    accessTokenExpiresAt = result.AccessTokenExpiresAt,
                    username = result.User.Username,
                    role = result.User.Role
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }
        }

        [HttpPost("refresh")]
        [EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                return Ok(await _authService.RefreshAsync(request.RefreshToken));
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
        {
            await _authService.RevokeRefreshTokenAsync(CurrentUserId(), request.RefreshToken);
            return NoContent();
        }


        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                await _authService.Register(registerDto);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("password-reset/request")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> RequestPasswordReset(PasswordResetRequest request)
        {
            await _authService.RequestPasswordResetAsync(request.Username);
            return Ok(new { message = "If the account exists, password reset instructions will be sent." });
        }

        [HttpPost("password-reset/confirm")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> ConfirmPasswordReset(PasswordResetConfirmRequest request)
        {
            var reset = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
            return reset ? NoContent() : BadRequest(new { message = "Invalid or expired password reset token." });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me() => Ok(new
        {
            id = User.FindFirstValue(ClaimTypes.NameIdentifier),
            username = User.FindFirstValue(ClaimTypes.Name),
            role = User.FindFirstValue(ClaimTypes.Role) ?? "client"
        });

        private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    }
}
