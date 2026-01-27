using Catalog.API.Repositories;
using Catalog.Domain;
using IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Catalog.API.Hubs;

namespace Catalog.API.Consumers;

public class PlateReservedConsumer : IConsumer<PlateReservedIntegrationEvent>
{
    private readonly IPlatesWatchlistRepository _watchRepo;
    private readonly INotificationsRepository _notifRepo;
    private readonly IHubContext<PlatesHub> _hub;
    private readonly IPublishEndpoint _publish;
    private readonly ILogger<PlateReservedConsumer> _logger;

    public PlateReservedConsumer(
        IPlatesWatchlistRepository watchRepo,
        INotificationsRepository notifRepo,
        IHubContext<PlatesHub> hub,
        IPublishEndpoint publish,
        ILogger<PlateReservedConsumer> logger)
    {
        _watchRepo = watchRepo;
        _notifRepo = notifRepo;
        _hub = hub;
        _publish = publish;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PlateReservedIntegrationEvent> context)
    {
        var ev = context.Message;
        var watch = await _watchRepo.GetByPlateIdAsync(ev.PlateId);
        if (watch == null) return;

        // State change => notify
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            PlateId = ev.PlateId,
            Title = "Plate Reserved",
            Message = $"Plate {ev.Registration} has been reserved at £{ev.SalePrice:N2}",
            CreatedDate = DateTime.UtcNow
        };

        await _notifRepo.AddAsync(notification);
        await _notifRepo.SaveChangesAsync();

        // push to group
        await _hub.Clients.Group(ev.PlateId.ToString()).SendAsync("NotificationReceived", new
        {
            notification.Id,
            notification.PlateId,
            notification.Title,
            notification.Message,
            notification.CreatedDate
        });

        _logger.LogInformation("Notified watchlist user about reserved plate {PlateId}", ev.PlateId);

        // Check price alert condition (if watch has PriceAlert and reserved price <= alert)
        if (watch.PriceAlert.HasValue && ev.SalePrice <= watch.PriceAlert.Value)
        {
            await _publish.Publish(new PriceAlertTriggeredIntegrationEvent
            {
                Id = Guid.NewGuid(),
                PlateId = ev.PlateId,
                Registration = ev.Registration,
                CurrentPrice = ev.SalePrice,
                AlertPrice = watch.PriceAlert,
                Reason = "Reserved price <= alert",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
