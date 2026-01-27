using Catalog.API.Repositories;
using Catalog.Domain;

namespace Catalog.API.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationsRepository _repo;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(INotificationsRepository repo, ILogger<NotificationService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<Notification>> GetNotificationsAsync()
    {
        return await _repo.GetAllAsync();
    }

    public async Task<Notification> CreateNotificationAsync(Notification notification)
    {
        await _repo.AddAsync(notification);
        await _repo.SaveChangesAsync();
        _logger.LogInformation("Created notification for plate {PlateId}", notification.PlateId);
        return notification;
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        var n = await _repo.GetByIdAsync(notificationId)
            ?? throw new KeyNotFoundException($"Notification {notificationId} not found");
        n.IsRead = true;
        await _repo.SaveChangesAsync();
    }
}
