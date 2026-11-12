using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Route("api/coupons")]
public sealed class CouponsController(CouponService coupons) : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> GetAll() => Ok(await coupons.GetAllAsync());

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Create(CouponRequest request)
    {
        try { return Ok(await coupons.CreateAsync(request)); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    [HttpPost("validate")]
    [Authorize]
    public async Task<IActionResult> Validate(CouponValidationRequest request, [FromQuery] decimal subtotal)
    {
        try { return Ok(await coupons.ValidateAsync(CurrentUserId(), request.Code, subtotal)); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
