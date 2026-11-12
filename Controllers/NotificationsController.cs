using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize]
[Route("api/notifications")]
public sealed class NotificationsController(NotificationService notifications) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await notifications.GetAsync(CurrentUserId()));

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount() => Ok(await notifications.GetUnreadCountAsync(CurrentUserId()));

    [HttpPatch("{id:int}/read")]
    public async Task<IActionResult> MarkRead(int id) => await notifications.MarkReadAsync(CurrentUserId(), id) ? NoContent() : NotFound();

    [HttpPatch("read-all")]
    public async Task<IActionResult> MarkAllRead()
    {
        await notifications.MarkAllReadAsync(CurrentUserId());
        return NoContent();
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
