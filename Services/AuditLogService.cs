using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;
using OnlineStoreBackend.Models;

namespace OnlineStoreBackend.Services;

public sealed class AuditLogService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<AuditLogResponse>> GetAsync(AuditLogQuery query)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);
        var logs = context.AuditLogs.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(query.Action)) logs = logs.Where(log => log.Action == query.Action);
        if (!string.IsNullOrWhiteSpace(query.EntityName)) logs = logs.Where(log => log.EntityName == query.EntityName);
        if (query.UserId.HasValue) logs = logs.Where(log => log.UserId == query.UserId.Value);
        return await logs.OrderByDescending(log => log.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize)
            .Select(log => new AuditLogResponse
            {
                Id = log.Id, UserId = log.UserId, Action = log.Action, EntityName = log.EntityName,
                EntityId = log.EntityId, Details = log.Details, IpAddress = log.IpAddress, CreatedAt = log.CreatedAt
            }).ToListAsync();
    }

    public async Task RecordAsync(int? userId, string action, string entityName, int? entityId, string? details, string? ipAddress)
    {
        context.AuditLogs.Add(new AuditLog
        {
            UserId = userId, Action = action, EntityName = entityName, EntityId = entityId,
            Details = details, IpAddress = ipAddress
        });
        await context.SaveChangesAsync();
    }
}
