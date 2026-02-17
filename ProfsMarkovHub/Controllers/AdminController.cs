using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;
using ProfsMarkovHub.Models;
using ProfsMarkovHub.Services.Storage;
using ProfsMarkovHub.ViewModels;

namespace ProfsMarkovHub.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAssetStorage _assetStorage;

    public AdminController(ApplicationDbContext context, IAssetStorage assetStorage)
    {
        _context = context;
        _assetStorage = assetStorage;
    }

    public async Task<IActionResult> Index()
    {
        var articles = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();
        return View(articles);
    }

    public IActionResult Create()
    {
        return View(new ArticleFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ArticleFormViewModel vm, IFormFile? imageFile)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var article = new Article
        {
            Title = vm.Title,
            Slug = vm.Slug,
            Content = vm.Content,
            Excerpt = vm.Excerpt,
            ImageUrl = vm.ImageUrl,
            PublishedAt = DateTime.UtcNow,
            AuthorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
        };

        if (imageFile is { Length: > 0 })
        {
            await using var stream = imageFile.OpenReadStream();
            article.ImageUrl = await _assetStorage.SaveAsync(stream, imageFile.FileName, imageFile.ContentType);
        }

        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        await SyncTags(article.Id, vm.TagsCsv);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var article = await _context.Articles
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Id == id);

        if (article == null) return NotFound();

        var vm = new ArticleFormViewModel
        {
            Id = article.Id,
            Title = article.Title,
            Slug = article.Slug,
            Content = article.Content,
            Excerpt = article.Excerpt,
            ImageUrl = article.ImageUrl,
            PublishedAt = article.PublishedAt,
            AuthorId = article.AuthorId,
            TagsCsv = string.Join(", ", article.ArticleTags.Select(at => at.Tag?.Name ?? ""))
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ArticleFormViewModel vm, IFormFile? imageFile)
    {
        if (id != vm.Id) return NotFound();
        if (!ModelState.IsValid) return View(vm);

        var article = await _context.Articles.FindAsync(id);
        if (article == null) return NotFound();

        article.Title = vm.Title;
        article.Slug = vm.Slug;
        article.Content = vm.Content;
        article.Excerpt = vm.Excerpt;
        article.ImageUrl = vm.ImageUrl;
        article.PublishedAt = vm.PublishedAt;
        article.AuthorId = vm.AuthorId;

        if (imageFile is { Length: > 0 })
        {
            await using var stream = imageFile.OpenReadStream();
            article.ImageUrl = await _assetStorage.SaveAsync(stream, imageFile.FileName, imageFile.ContentType);
        }

        await _context.SaveChangesAsync();
        await SyncTags(article.Id, vm.TagsCsv);

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var article = await _context.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (article == null) return NotFound();
        return View(article);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var article = await _context.Articles.FindAsync(id);
        if (article != null)
        {
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // ---- Helpers ----

    private async Task SyncTags(int articleId, string? tagsCsv)
    {
        // Remove existing
        var existing = await _context.ArticleTags.Where(at => at.ArticleId == articleId).ToListAsync();
        _context.ArticleTags.RemoveRange(existing);

        if (string.IsNullOrWhiteSpace(tagsCsv))
        {
            await _context.SaveChangesAsync();
            return;
        }

        var tagNames = tagsCsv
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var name in tagNames)
        {
            var slug = name.ToLowerInvariant().Replace(' ', '-');
            var tag = await _context.Tags.FirstOrDefaultAsync(t => t.Slug == slug);
            if (tag == null)
            {
                tag = new Tag { Name = name, Slug = slug };
                _context.Tags.Add(tag);
                await _context.SaveChangesAsync();
            }

            _context.ArticleTags.Add(new ArticleTag { ArticleId = articleId, TagId = tag.Id });
        }

        await _context.SaveChangesAsync();
    }
}
