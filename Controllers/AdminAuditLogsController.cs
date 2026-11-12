using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Services;

namespace OnlineStoreBackend.Controllers;

[ApiController]
[Authorize(Roles = "admin")]
[Route("api/admin/audit-logs")]
public sealed class AdminAuditLogsController(AuditLogService logs) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] AuditLogQuery query) => Ok(await logs.GetAsync(query));
}
