using Catalog.Domain;

namespace Catalog.API.Repositories;

public interface IPlatesWatchlistRepository
{
    Task<List<PlatesWatchlist>> GetAllAsync();
    Task<PlatesWatchlist?> GetByPlateIdAsync(Guid plateId);
    Task AddAsync(PlatesWatchlist watch);
    Task RemoveAsync(PlatesWatchlist watch);
    Task<bool> ExistsAsync(Guid plateId);
    Task SaveChangesAsync();
}
