using Catalog.API.Repositories;
using Catalog.Domain;

namespace Catalog.API.Services;

public class PlatesWatchlistService : IPlatesWatchlistService
{
    private readonly IPlatesWatchlistRepository _repository;

    public PlatesWatchlistService(IPlatesWatchlistRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<PlatesWatchlist>> GetWatchlistAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<PlatesWatchlist> AddToWatchlistAsync(Guid plateId, decimal? priceAlert)
    {
        if (await _repository.ExistsAsync(plateId))
            throw new InvalidOperationException($"Plate {plateId} is already in the watchlist");

        var watch = new PlatesWatchlist
        {
            PlateId = plateId,
            PriceAlert = priceAlert
        };

        await _repository.AddAsync(watch);
        await _repository.SaveChangesAsync();

        return watch;
    }

    public async Task RemoveFromWatchlistAsync(Guid plateId)
    {
        var watch = await _repository.GetByPlateIdAsync(plateId);
        if (watch == null)
            throw new KeyNotFoundException($"Plate {plateId} is not in the watchlist");

        await _repository.RemoveAsync(watch);
        await _repository.SaveChangesAsync();
    }
}
