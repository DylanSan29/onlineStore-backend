using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/reviews")]
public sealed class AdminReviewsController(ReviewService reviews) : ControllerBase
{
    [HttpGet("pending")]
    public async Task<IActionResult> GetPending() => Ok(await reviews.GetPendingAsync());

    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id) => await reviews.ApproveAsync(id) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) => await reviews.DeleteAsync(id) ? NoContent() : NotFound();
}
