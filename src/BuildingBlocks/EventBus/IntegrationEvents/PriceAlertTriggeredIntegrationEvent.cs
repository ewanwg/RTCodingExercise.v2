namespace IntegrationEvents;

public class PriceAlertTriggeredIntegrationEvent : IntegrationEvent
{
    public Guid PlateId { get; set; }
    public string Registration { get; set; } = string.Empty;
    public decimal CurrentPrice { get; set; }
    public decimal? AlertPrice { get; set; }   // Null => generic alert
    public string Reason { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
