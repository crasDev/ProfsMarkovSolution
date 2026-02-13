using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;
using ProfsMarkovHub.Models.Store;
using ProfsMarkovHub.Services.Store;
using ProfsMarkovHub.ViewModels;

namespace ProfsMarkovHub.Controllers;

public class StoreController : Controller
{
    private readonly IStreamElementsService _streamElementsService;
    private readonly ApplicationDbContext _context;
    private readonly ILogger<StoreController> _logger;

    public StoreController(IStreamElementsService streamElementsService, ApplicationDbContext context, ILogger<StoreController> logger)
    {
        _streamElementsService = streamElementsService;
        _context = context;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        try
        {
            var items = await _streamElementsService.GetStoreItemsAsync(cancellationToken);

            foreach (var item in items)
            {
                var existing = await _context.StoreItems.FirstOrDefaultAsync(x => x.ExternalId == item.ExternalId, cancellationToken);
                if (existing == null)
                {
                    _context.StoreItems.Add(item);
                }
                else
                {
                    existing.Name = item.Name;
                    existing.Description = item.Description;
                    existing.ImageUrl = item.ImageUrl;
                    existing.Cost = item.Cost;
                    existing.IsActive = item.IsActive;
                    existing.LastSyncedAt = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return View(new StoreViewModel
            {
                Items = items.Where(x => x.IsActive).OrderBy(x => x.Cost).ToList(),
                IsFallback = false
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Falling back to local mock store data");
            var fallbackItems = await _context.StoreItems
                .Where(x => x.IsActive)
                .OrderBy(x => x.Cost)
                .Take(12)
                .ToListAsync(cancellationToken);

            if (!fallbackItems.Any())
            {
                fallbackItems = new List<StoreItem>
                {
                    new() { ExternalId = "mock-1", Name = "Random Steam Key", Description = "Mystery key drop for your library.", Cost = 7500, ImageUrl = "/images/store-placeholder.svg", IsActive = true },
                    new() { ExternalId = "mock-2", Name = "Xbox Game Pass (1 Month)", Description = "Keep gaming with a month of Game Pass.", Cost = 18000, ImageUrl = "/images/store-placeholder.svg", IsActive = true },
                    new() { ExternalId = "mock-3", Name = "Steam Wallet €20", Description = "Top up your Steam wallet instantly.", Cost = 20000, ImageUrl = "/images/store-placeholder.svg", IsActive = true }
                };
            }

            return View(new StoreViewModel
            {
                Items = fallbackItems,
                IsFallback = true,
                Message = "Live StreamElements data is currently unavailable. Showing cached/mock rewards."
            });
        }
    }
}
