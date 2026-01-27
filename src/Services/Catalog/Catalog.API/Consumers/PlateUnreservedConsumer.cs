using Catalog.API.Repositories;
using Catalog.Domain;
using IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Catalog.API.Hubs;

namespace Catalog.API.Consumers;

public class PlateUnreservedConsumer : IConsumer<PlateUnreservedIntegrationEvent>
{
    private readonly IPlatesWatchlistRepository _watchRepo;
    private readonly INotificationsRepository _notifRepo;
    private readonly IHubContext<PlatesHub> _hub;
    private readonly ILogger<PlateUnreservedConsumer> _logger;

    public PlateUnreservedConsumer(
        IPlatesWatchlistRepository watchRepo,
        INotificationsRepository notifRepo,
        IHubContext<PlatesHub> hub,
        ILogger<PlateUnreservedConsumer> logger)
    {
        _watchRepo = watchRepo;
        _notifRepo = notifRepo;
        _hub = hub;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PlateUnreservedIntegrationEvent> context)
    {
        var ev = context.Message;
        var watch = await _watchRepo.GetByPlateIdAsync(ev.PlateId);
        if (watch == null) return;

        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            PlateId = ev.PlateId,
            Title = "Plate Unreserved",
            Message = $"Plate {ev.Registration} has been unreserved",
            CreatedDate = DateTime.UtcNow
        };

        await _notifRepo.AddAsync(notification);
        await _notifRepo.SaveChangesAsync();

        await _hub.Clients.Group(ev.PlateId.ToString()).SendAsync("NotificationReceived", new
        {
            notification.Id,
            notification.PlateId,
            notification.Title,
            notification.Message,
            notification.CreatedDate
        });

        _logger.LogInformation("Notified watchlist user about unreserved plate {PlateId}", ev.PlateId);
    }
}
