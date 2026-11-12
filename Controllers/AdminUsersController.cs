using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/users")]
public sealed class AdminUsersController(AdminUserService users) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await users.GetAsync());

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id) => (await users.GetByIdAsync(id)) is { } user ? Ok(user) : NotFound();

    [HttpPatch("{id:int}/role")]
    public async Task<IActionResult> UpdateRole(int id, UpdateUserRoleRequest request)
    {
        try { return await users.UpdateRoleAsync(CurrentUserId(), id, request.Role) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromQuery] bool isActive)
    {
        try { return await users.UpdateStatusAsync(CurrentUserId(), id, isActive) ? NoContent() : NotFound(); }
        catch (InvalidOperationException exception) { return BadRequest(new { message = exception.Message }); }
    }

    private int CurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
