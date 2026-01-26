using Catalog.Domain;

namespace Catalog.API.Services;

public interface IPlatesWatchlistService
{
    Task<List<PlatesWatchlist>> GetWatchlistAsync();
    Task<PlatesWatchlist> AddToWatchlistAsync(Guid plateId, decimal? priceAlert);
    Task RemoveFromWatchlistAsync(Guid plateId);
}
