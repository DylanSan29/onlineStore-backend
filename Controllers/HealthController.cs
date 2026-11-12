using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Route("health")]
public sealed class HealthController(OnlineStoreContext context) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok(new HealthResponse { Status = "Healthy", Service = "OnlineStoreBackend", CheckedAt = DateTime.UtcNow });

    [HttpGet("database")]
    public async Task<IActionResult> Database()
    {
        var connected = await context.Database.CanConnectAsync();
        var response = new HealthResponse
        {
            Status = connected ? "Healthy" : "Unhealthy", Service = "OnlineStoreDatabase", CheckedAt = DateTime.UtcNow
        };
        return connected ? Ok(response) : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
