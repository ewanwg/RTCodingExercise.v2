using Catalog.API.Repositories;
using Catalog.Domain;
using IntegrationEvents;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Catalog.API.Hubs;

namespace Catalog.API.Consumers;

public class PlateSoldConsumer : IConsumer<PlateSoldIntegrationEvent>
{
    private readonly IPlatesWatchlistRepository _watchRepo;
    private readonly INotificationsRepository _notifRepo;
    private readonly IHubContext<PlatesHub> _hub;
    private readonly IPublishEndpoint _publish;
    private readonly ILogger<PlateSoldConsumer> _logger;

    public PlateSoldConsumer(
        IPlatesWatchlistRepository watchRepo,
        INotificationsRepository notifRepo,
        IHubContext<PlatesHub> hub,
        IPublishEndpoint publish,
        ILogger<PlateSoldConsumer> logger)
    {
        _watchRepo = watchRepo;
        _notifRepo = notifRepo;
        _hub = hub;
        _publish = publish;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PlateSoldIntegrationEvent> context)
    {
        var ev = context.Message;
        var watch = await _watchRepo.GetByPlateIdAsync(ev.PlateId);
        if (watch == null) return;

        // State change => notify
        var notification = new Notification
        {
            Id = Guid.NewGuid(),
            PlateId = ev.PlateId,
            Title = "Plate Sold",
            Message = $"Plate {ev.Registration} has been sold for £{ev.SoldPrice:N2}",
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

        _logger.LogInformation("Notified watchlist user about sold plate {PlateId}", ev.PlateId);

        // Price alert check (if alert exists and sold price <= alert)
        if (watch.PriceAlert.HasValue && ev.SoldPrice <= watch.PriceAlert.Value)
        {
            await _publish.Publish(new PriceAlertTriggeredIntegrationEvent
            {
                Id = Guid.NewGuid(),
                PlateId = ev.PlateId,
                Registration = ev.Registration,
                CurrentPrice = ev.SoldPrice,
                AlertPrice = watch.PriceAlert,
                Reason = "Sold price <= alert",
                CreatedAt = DateTime.UtcNow
            });
        }
    }
}
