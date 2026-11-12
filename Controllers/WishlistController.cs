using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/users/me/wishlist")]
public sealed class WishlistController(WishlistService wishlist) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await wishlist.GetAsync(CurrentUserId()));

    [HttpPost("{productId:int}")]
    public async Task<IActionResult> Add(int productId)
    {
        var item = await wishlist.AddAsync(CurrentUserId(), productId);
        return item is null ? NotFound(new { message = "Product not found" }) : Ok(item);
    }

    [HttpDelete("{productId:int}")]
    public async Task<IActionResult> Remove(int productId) =>
        await wishlist.RemoveAsync(CurrentUserId(), productId) ? NoContent() : NotFound();

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
