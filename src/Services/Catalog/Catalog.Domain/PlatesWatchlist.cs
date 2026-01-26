namespace Catalog.Domain;

public class PlatesWatchlist
{
    public Guid Id { get; set; }
    public Guid PlateId { get; set; }
    public decimal? PriceAlert { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public Plate? Plate { get; set; }
}
