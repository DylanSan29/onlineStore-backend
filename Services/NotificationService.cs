using Microsoft.EntityFrameworkCore;
using OnlineStoreBackend.Data;
using OnlineStoreBackend.DTOs;

namespace OnlineStoreBackend.Services;

public sealed class NotificationService(OnlineStoreContext context)
{
    public async Task<IReadOnlyList<NotificationResponse>> GetAsync(int userId) =>
        await context.Notifications.AsNoTracking().Where(item => item.UserId == userId)
            .OrderByDescending(item => item.CreatedAt).Select(item => new NotificationResponse
            {
                Id = item.Id, Type = item.Type, Title = item.Title, Message = item.Message,
                IsRead = item.IsRead, ReadAt = item.ReadAt, CreatedAt = item.CreatedAt
            }).ToListAsync();

    public async Task<NotificationCountResponse> GetUnreadCountAsync(int userId) =>
        new() { UnreadCount = await context.Notifications.CountAsync(item => item.UserId == userId && !item.IsRead) };

    public async Task<bool> MarkReadAsync(int userId, int notificationId)
    {
        var notification = await context.Notifications.FirstOrDefaultAsync(item => item.Id == notificationId && item.UserId == userId);
        if (notification is null) return false;
        notification.IsRead = true;
        notification.ReadAt ??= DateTime.UtcNow;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task MarkAllReadAsync(int userId)
    {
        var notifications = await context.Notifications.Where(item => item.UserId == userId && !item.IsRead).ToListAsync();
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }
        await context.SaveChangesAsync();
    }
}
