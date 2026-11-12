using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Route("api/products/{productId:int}/reviews")]
public sealed class ReviewsController(ReviewService reviews) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Get(int productId) => Ok(await reviews.GetApprovedAsync(productId));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(int productId, ReviewRequest request)
    {
        try { return Ok(await reviews.CreateAsync(CurrentUserId(), productId, request)); }
        catch (KeyNotFoundException exception) { return NotFound(new { message = exception.Message }); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
