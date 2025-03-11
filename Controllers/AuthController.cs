using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

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
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var (token, user) = await _authService.Login(loginDto);  // Deconstruct the tuple

            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new
            {
                token,
                username = user.Username,
                role = user.Role
            });
        }


        [HttpPost("register")]
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

    }
}
