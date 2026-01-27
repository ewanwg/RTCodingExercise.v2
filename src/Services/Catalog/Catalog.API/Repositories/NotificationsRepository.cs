using Catalog.API.Data;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories;

public interface INotificationsRepository
{
    Task<List<Notification>> GetAllAsync();
    Task AddAsync(Notification notification);
    Task<Notification?> GetByIdAsync(Guid id);
    Task SaveChangesAsync();
}

public class NotificationsRepository : INotificationsRepository
{
    private readonly ApplicationDbContext _context;

    public NotificationsRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Notification>> GetAllAsync()
    {
        return await _context.Set<Notification>()
            .Include(n => n.Plate)
            .OrderByDescending(n => n.CreatedDate)
            .ToListAsync();
    }

    public async Task AddAsync(Notification notification)
    {
        await _context.Set<Notification>().AddAsync(notification);
    }

    public async Task<Notification?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Notification>().FindAsync(id);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
