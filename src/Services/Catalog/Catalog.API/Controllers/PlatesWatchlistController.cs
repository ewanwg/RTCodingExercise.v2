using Catalog.API.Services;
using Catalog.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlatesWatchlistController : ControllerBase
{
    private readonly IPlatesWatchlistService _watchlistService;
    private readonly ILogger<PlatesWatchlistController> _logger;

    public PlatesWatchlistController(
        IPlatesWatchlistService watchlistService,
        ILogger<PlatesWatchlistController> logger)
    {
        _watchlistService = watchlistService;
        _logger = logger;
    }

    /// <summary>
    /// Get the full plates watchlist
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PlatesWatchlist>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PlatesWatchlist>>> GetWatchlist()
    {
        var list = await _watchlistService.GetWatchlistAsync();
        return Ok(list);
    }

    /// <summary>
    /// Add a plate to the watchlist
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PlatesWatchlist), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PlatesWatchlist>> AddToWatchlist([FromBody] PlatesWatchlist request)
    {
        try
        {
            var added = await _watchlistService.AddToWatchlistAsync(request.PlateId, request.PriceAlert);
            _logger.LogInformation("Added plate {PlateId} to watchlist", request.PlateId);
            return Ok(added);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to add plate {PlateId} to watchlist: {Error}", request.PlateId, ex.Message);
            return BadRequest(new { error = ex.Message });
        }
    }

    /// <summary>
    /// Remove a plate from the watchlist
    /// </summary>
    [HttpDelete("{plateId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromWatchlist(Guid plateId)
    {
        try
        {
            await _watchlistService.RemoveFromWatchlistAsync(plateId);
            _logger.LogInformation("Removed plate {PlateId} from watchlist", plateId);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning("Attempted to remove plate {PlateId} which is not in the watchlist: {Error}", plateId, ex.Message);
            return NotFound();
        }
    }
}
