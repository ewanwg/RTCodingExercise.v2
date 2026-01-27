namespace Catalog.Domain;

public class Notification
{
    public Guid Id { get; set; }
    public Guid? PlateId { get; set; }         
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // Navigation
    public Plate? Plate { get; set; }
}
