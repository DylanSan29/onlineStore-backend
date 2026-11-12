using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Services;
using System.Security.Claims;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/reviews")]
public sealed class AdminReviewsController(ReviewService reviews) : ControllerBase
{
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending() => Ok(await reviews.GetPendingAsync());

    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id) => await reviews.ApproveAsync(CurrentUserId(), id) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => await reviews.DeleteAsync(CurrentUserId(), id) ? NoContent() : NotFound();

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
