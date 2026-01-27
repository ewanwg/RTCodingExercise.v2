using Catalog.Domain;

namespace Catalog.API.Services;

public interface INotificationService
{
    Task<List<Notification>> GetNotificationsAsync();
    Task<Notification> CreateNotificationAsync(Notification notification);
    Task MarkAsReadAsync(Guid notificationId);
}
