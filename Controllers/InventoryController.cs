using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/inventory")]
public sealed class InventoryController(InventoryService inventory) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int threshold = 5) => Ok(await inventory.GetAllAsync(Math.Max(0, threshold)));

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock([FromQuery] int threshold = 5) => Ok(await inventory.GetLowStockAsync(Math.Max(0, threshold)));

    [HttpGet("movements")]
    public async Task<IActionResult> GetMovements() => Ok(await inventory.GetMovementsAsync());

    [HttpPost("adjustments")]
    public async Task<IActionResult> Adjust(InventoryAdjustmentRequest request)
    {
        try { return Ok(await inventory.AdjustAsync(CurrentUserId(), request)); }
        catch (KeyNotFoundException exception) { return NotFound(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
