using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/notifications")]
public sealed class AdminNotificationsController(NotificationService notifications) : ControllerBase
{
    [HttpPost("users/{userId:int}")]
    public async Task<IActionResult> Create(int userId, CreateNotificationRequest request) =>
        Ok(await notifications.CreateAsync(userId, request));
}
