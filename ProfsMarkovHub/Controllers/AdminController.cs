using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;
using ProfsMarkovHub.Models;
using ProfsMarkovHub.Services;
using ProfsMarkovHub.Services.Storage;
using ProfsMarkovHub.ViewModels;

namespace ProfsMarkovHub.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IAssetStorage _assetStorage;
    private readonly ISlugService _slugService;
    private readonly IPublishService _publishService;

    public AdminController(ApplicationDbContext context, IAssetStorage assetStorage, ISlugService slugService, IPublishService publishService)
    {
        _context = context;
        _assetStorage = assetStorage;
        _slugService = slugService;
        _publishService = publishService;
    }

    public async Task<IActionResult> Index(string? status, string? tag, string? q, int page = 1, int pageSize = 10)
    {
        var query = _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .Where(a => !a.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ArticleStatus>(status, true, out var st))
            query = query.Where(a => a.Status == st);

        if (!string.IsNullOrWhiteSpace(tag))
            query = query.Where(a => a.ArticleTags.Any(at => at.Tag != null && (at.Tag.Name == tag || at.Tag.Slug == tag)));

        if (!string.IsNullOrWhiteSpace(q))
            query = query.Where(a => a.Title.Contains(q) || a.Slug.Contains(q));

        query = query.OrderByDescending(a => a.PublishedAt).ThenByDescending(a => a.Id);

        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        ViewBag.Page = page;
        ViewBag.PageSize = pageSize;
        ViewBag.Status = status;
        ViewBag.Tag = tag;
        ViewBag.Q = q;
        return View(items);
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

        // Canonicalize/validate slug
        if (string.IsNullOrWhiteSpace(vm.Slug)) vm.Slug = _slugService.GenerateSlug(vm.Title);
        else vm.Slug = _slugService.GenerateSlug(vm.Slug);
        if (!await _slugService.IsUniqueAsync(vm.Slug))
        {
            ModelState.AddModelError("Slug", "Slug already exists. Please choose another.");
            return View(vm);
        }

        var article = new Article
        {
            Title = vm.Title,
            Slug = vm.Slug,
            Content = vm.Content,
            Excerpt = vm.Excerpt,
            ImageUrl = vm.ImageUrl,
            AuthorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            OgTitle = vm.OgTitle,
            OgDescription = vm.OgDescription,
            OgImageUrl = vm.OgImageUrl
        };

        _publishService.ApplyStatusTransition(article, vm.Status, vm.ScheduledAt);

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
            Status = article.Status,
            PublishedAt = article.PublishedAt,
            ScheduledAt = article.ScheduledAt,
            AuthorId = article.AuthorId,
            OgTitle = article.OgTitle,
            OgDescription = article.OgDescription,
            OgImageUrl = article.OgImageUrl,
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

        // Slug validation
        if (string.IsNullOrWhiteSpace(vm.Slug)) vm.Slug = _slugService.GenerateSlug(vm.Title);
        else vm.Slug = _slugService.GenerateSlug(vm.Slug);
        if (!await _slugService.IsUniqueAsync(vm.Slug, excludingArticleId: id))
        {
            ModelState.AddModelError("Slug", "Slug already exists. Please choose another.");
            return View(vm);
        }

        article.Title = vm.Title;
        article.Slug = vm.Slug;
        article.Content = vm.Content;
        article.Excerpt = vm.Excerpt;
        article.ImageUrl = vm.ImageUrl;
        article.AuthorId = vm.AuthorId;
        article.OgTitle = vm.OgTitle;
        article.OgDescription = vm.OgDescription;
        article.OgImageUrl = vm.OgImageUrl;

        _publishService.ApplyStatusTransition(article, vm.Status, vm.ScheduledAt);

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
            article.IsDeleted = true;
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Bulk(string actionType, int[] selectedIds)
    {
        if (selectedIds == null || selectedIds.Length == 0) return RedirectToAction(nameof(Index));
        var items = await _context.Articles.Where(a => selectedIds.Contains(a.Id)).ToListAsync();
        foreach (var a in items)
        {
            switch (actionType)
            {
                case "Publish":
                    _publishService.ApplyStatusTransition(a, ArticleStatus.Published, null);
                    break;
                case "Unpublish":
                    _publishService.ApplyStatusTransition(a, ArticleStatus.Draft, null);
                    break;
                case "Delete":
                    a.IsDeleted = true;
                    break;
            }
        }
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult AutoPublish()
    {
        var count = _publishService.AutoPublishDue();
        TempData["Message"] = $"Auto-published {count} scheduled articles.";
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
