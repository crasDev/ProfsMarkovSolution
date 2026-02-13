using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;
using ProfsMarkovHub.Models;
using ProfsMarkovHub.Services.Storage;

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
        return View(await _context.Articles.Include(a => a.Author).ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Title,Slug,Content,ImageUrl")] Article article, IFormFile? imageFile)
    {
        if (ModelState.IsValid)
        {
            if (imageFile is { Length: > 0 })
            {
                await using var stream = imageFile.OpenReadStream();
                article.ImageUrl = await _assetStorage.SaveAsync(stream, imageFile.FileName, imageFile.ContentType);
            }

            article.PublishedAt = DateTime.UtcNow;
            article.AuthorId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            _context.Add(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(article);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = await _context.Articles.FindAsync(id);
        if (article == null)
        {
            return NotFound();
        }
        return View(article);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Slug,Content,ImageUrl,PublishedAt,AuthorId")] Article article, IFormFile? imageFile)
    {
        if (id != article.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                if (imageFile is { Length: > 0 })
                {
                    await using var stream = imageFile.OpenReadStream();
                    article.ImageUrl = await _assetStorage.SaveAsync(stream, imageFile.FileName, imageFile.ContentType);
                }

                _context.Update(article);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(article.Id))
                {
                    return NotFound();
                }
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(article);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = await _context.Articles
            .Include(a => a.Author)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (article == null)
        {
            return NotFound();
        }

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
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ArticleExists(int id)
    {
        return _context.Articles.Any(e => e.Id == id);
    }
}
