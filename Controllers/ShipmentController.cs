using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/orders/{orderId:int}/shipment")]
public sealed class ShipmentController(ShipmentService shipments) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(int orderId)
    {
        var shipment = await shipments.GetForOrderAsync(CurrentUserId(), User.IsInRole("admin"), orderId);
        return shipment is null ? NotFound() : Ok(shipment);
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
