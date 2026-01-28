using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Repositories;
using Catalog.API.Services;
using Catalog.Domain;
using Xunit;

namespace Catalog.UnitTests
{
    public class PlatesWatchlistServiceTests
    {
        [Fact]
        public async Task GetWatchlistAsync_ShouldReturnAllItems()
        {
            // Arrange
            var repo = new FakePlatesWatchlistRepository(new[]
            {
                new PlatesWatchlist { Id = Guid.NewGuid(), PlateId = Guid.NewGuid(), PriceAlert = 100m, CreatedDate = DateTime.UtcNow.AddMinutes(-1) },
                new PlatesWatchlist { Id = Guid.NewGuid(), PlateId = Guid.NewGuid(), PriceAlert = 200m, CreatedDate = DateTime.UtcNow }
            });
            var service = new PlatesWatchlistService(repo);

            // Act
            var result = await service.GetWatchlistAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Contains(result, r => r.PriceAlert == 100m);
            Assert.Contains(result, r => r.PriceAlert == 200m);
        }

        [Fact]
        public async Task AddToWatchlistAsync_WhenNotExists_ShouldAddAndReturnWatch()
        {
            // Arrange
            var repo = new FakePlatesWatchlistRepository();
            var service = new PlatesWatchlistService(repo);
            var plateId = Guid.NewGuid();

            // Act
            var added = await service.AddToWatchlistAsync(plateId, 150.50m);

            // Assert
            Assert.NotNull(added);
            Assert.Equal(plateId, added.PlateId);
            Assert.Equal(150.50m, added.PriceAlert);
            Assert.True(repo.AddCalled);
            Assert.True(repo.SaveCalled);
            Assert.Single(repo.Store);
            Assert.Equal(plateId, repo.Store.Single().PlateId);
        }

        [Fact]
        public async Task AddToWatchlistAsync_WhenAlreadyExists_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var existing = new PlatesWatchlist { Id = Guid.NewGuid(), PlateId = plateId, PriceAlert = 99m };
            var repo = new FakePlatesWatchlistRepository(new[] { existing });
            var service = new PlatesWatchlistService(repo);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => service.AddToWatchlistAsync(plateId, 50m));
            Assert.Contains(plateId.ToString(), ex.Message);
            Assert.False(repo.SaveCalled); // Ensure we didn't save when it already existed
        }

        [Fact]
        public async Task RemoveFromWatchlistAsync_WhenExists_ShouldRemoveAndSave()
        {
            // Arrange
            var plateId = Guid.NewGuid();
            var existing = new PlatesWatchlist { Id = Guid.NewGuid(), PlateId = plateId, PriceAlert = 42m };
            var repo = new FakePlatesWatchlistRepository(new[] { existing });
            var service = new PlatesWatchlistService(repo);

            // Act
            await service.RemoveFromWatchlistAsync(plateId);

            // Assert
            Assert.True(repo.RemoveCalled);
            Assert.True(repo.SaveCalled);
            Assert.Empty(repo.Store);
        }

        [Fact]
        public async Task RemoveFromWatchlistAsync_WhenNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var repo = new FakePlatesWatchlistRepository();
            var service = new PlatesWatchlistService(repo);
            var plateId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.RemoveFromWatchlistAsync(plateId));
            Assert.False(repo.SaveCalled);
        }

        #region Fake repository for tests
        // Lightweight in-memory fake repository implementing IPlatesWatchlistRepository.
        // Keeps the tests free of external mocking frameworks and EF Core in-memory configuration.
        private class FakePlatesWatchlistRepository : IPlatesWatchlistRepository
        {
            public List<PlatesWatchlist> Store { get; } = new();
            public bool AddCalled { get; private set; }
            public bool RemoveCalled { get; private set; }
            public bool SaveCalled { get; private set; }

            public FakePlatesWatchlistRepository()
            {
            }

            public FakePlatesWatchlistRepository(IEnumerable<PlatesWatchlist> initial)
            {
                Store.AddRange(initial);
            }

            public Task<List<PlatesWatchlist>> GetAllAsync()
            {
                // Return copy to mimic repository behavior
                var result = Store.OrderByDescending(x => x.CreatedDate).ToList();
                return Task.FromResult(result);
            }

            public Task<PlatesWatchlist?> GetByPlateIdAsync(Guid plateId)
            {
                var item = Store.FirstOrDefault(x => x.PlateId == plateId);
                return Task.FromResult(item);
            }

            public Task AddAsync(PlatesWatchlist watch)
            {
                AddCalled = true;
                if (watch.Id == Guid.Empty)
                    watch.Id = Guid.NewGuid();
                if (watch.CreatedDate == default)
                    watch.CreatedDate = DateTime.UtcNow;
                Store.Add(watch);
                return Task.CompletedTask;
            }

            public Task RemoveAsync(PlatesWatchlist watch)
            {
                RemoveCalled = true;
                Store.Remove(watch);
                return Task.CompletedTask;
            }

            public Task<bool> ExistsAsync(Guid plateId)
            {
                return Task.FromResult(Store.Any(x => x.PlateId == plateId));
            }

            public Task SaveChangesAsync()
            {
                SaveCalled = true;
                return Task.CompletedTask;
            }
        }
        #endregion
    }
}
