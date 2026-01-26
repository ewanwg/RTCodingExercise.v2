using Catalog.API.Data;
using Catalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Repositories;

public class PlatesWatchlistRepository : IPlatesWatchlistRepository
{
    private readonly ApplicationDbContext _context;

    public PlatesWatchlistRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PlatesWatchlist>> GetAllAsync()
    {
        return await _context.PlatesWatchlist
            .Include(x => x.Plate)
            .OrderByDescending(x => x.CreatedDate)
            .ToListAsync();
    }

    public async Task<PlatesWatchlist?> GetByPlateIdAsync(Guid plateId)
    {
        return await _context.PlatesWatchlist
            .Include(x => x.Plate)
            .FirstOrDefaultAsync(x => x.PlateId == plateId);
    }

    public async Task AddAsync(PlatesWatchlist watch)
    {
        await _context.PlatesWatchlist.AddAsync(watch);
    }

    public async Task RemoveAsync(PlatesWatchlist watch)
    {
        _context.PlatesWatchlist.Remove(watch);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid plateId)
    {
        return await _context.PlatesWatchlist.AnyAsync(x => x.PlateId == plateId);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
