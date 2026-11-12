using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/orders")]
public sealed class OrdersController(OrderService orders) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create(CreateOrderRequest request)
    {
        try
        {
            var order = await orders.CreateAsync(CurrentUserId(), request);
            return CreatedAtAction(nameof(GetById), new { id = order.Id }, order);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMine() => Ok(await orders.GetForUserAsync(CurrentUserId()));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await orders.GetByIdAsync(CurrentUserId(), IsAdmin(), id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost("{id:int}/cancel")]
    public async Task<IActionResult> Cancel(int id) => await orders.CancelAsync(CurrentUserId(), IsAdmin(), id) ? NoContent() : BadRequest(new { message = "Order cannot be cancelled" });

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin() => User.IsInRole("admin");
}

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/orders")]
public sealed class AdminOrdersController(OrderService orders) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await orders.GetAllAsync());

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request)
    {
        try { return await orders.UpdateStatusAsync(id, request.Status) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }
}
