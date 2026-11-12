using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;

namespace OnlineStoreBackend.Services;

public sealed class AdminUserService(OnlineStoreContext context, AuditLogService? audit = null)
{
    public async Task<IReadOnlyList<UserAdminResponse>> GetAsync() =>
        await context.Users.AsNoTracking().OrderBy(user => user.Username).Select(user => new UserAdminResponse
        {
            Id = user.Id, Username = user.Username, Role = user.Role, IsActive = user.IsActive
        }).ToListAsync();

    public async Task<UserAdminResponse?> GetByIdAsync(int id) =>
        await context.Users.AsNoTracking().Where(user => user.Id == id).Select(user => new UserAdminResponse
        {
            Id = user.Id, Username = user.Username, Role = user.Role, IsActive = user.IsActive
        }).SingleOrDefaultAsync();

    public async Task<bool> UpdateRoleAsync(int actorId, int id, string role)
    {
        if (role is not ("admin" or "client")) throw new InvalidOperationException("Role must be admin or client.");
        var user = await context.Users.FindAsync(id);
        if (user is null) return false;
        user.Role = role;
        await context.SaveChangesAsync();
        if (audit is not null) await audit.RecordAsync(actorId, "UpdateRole", "User", id, $"Role changed to {role}", null);
        return true;
    }

    public async Task<bool> UpdateStatusAsync(int actorId, int id, bool isActive)
    {
        if (actorId == id && !isActive) throw new InvalidOperationException("Administrators cannot deactivate themselves.");
        var user = await context.Users.FindAsync(id);
        if (user is null) return false;
        user.IsActive = isActive;
        await context.SaveChangesAsync();
        if (audit is not null) await audit.RecordAsync(actorId, "UpdateStatus", "User", id, $"Active: {isActive}", null);
        return true;
    }

}
