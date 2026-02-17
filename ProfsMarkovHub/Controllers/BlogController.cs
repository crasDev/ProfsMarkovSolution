using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;
using ProfsMarkovHub.Helpers;

namespace ProfsMarkovHub.Controllers;

public class BlogController : Controller
{
    private readonly ApplicationDbContext _context;

    public BlogController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["OgTitle"] = "ProfsMarkov Blog";
        ViewData["OgDescription"] = "Latest posts, insights, and experiments from ProfsMarkov.";
        ViewData["OgImage"] = Url.Content("~/images/hero-bg.jpg");
        ViewData["OgUrl"] = Url.Action("Index", "Blog", null, Request.Scheme);

        var articles = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
            .OrderByDescending(a => a.PublishedAt)
            .ToListAsync();

        return View(articles);
    }

    [Route("blog/{slug}")]
    public async Task<IActionResult> DetailsBySlug(string slug)
    {
        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(a => a.Slug == slug);

        if (article == null)
            return NotFound();

        ViewData["Title"] = article.Title;
        ViewData["OgTitle"] = article.Title;
        ViewData["OgDescription"] = !string.IsNullOrEmpty(article.Excerpt) 
            ? article.Excerpt 
            : (article.Content.Length > 180 ? article.Content[..180] + "..." : article.Content);
        ViewData["OgImage"] = string.IsNullOrWhiteSpace(article.ImageUrl) 
            ? Url.Content("~/images/hero-bg.jpg") 
            : article.ImageUrl;
        ViewData["OgUrl"] = Url.Action("DetailsBySlug", "Blog", new { slug = article.Slug }, Request.Scheme);

        ViewData["RenderedContent"] = MarkdownHelper.ToHtml(article.Content);
        ViewData["ReadingTime"] = Math.Max(1, article.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length / 200);

        return View("Details", article);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (article == null)
            return NotFound();

        ViewData["Title"] = article.Title;
        ViewData["OgTitle"] = article.Title;
        ViewData["OgDescription"] = !string.IsNullOrEmpty(article.Excerpt) 
            ? article.Excerpt 
            : (article.Content.Length > 180 ? article.Content[..180] + "..." : article.Content);
        ViewData["OgImage"] = string.IsNullOrWhiteSpace(article.ImageUrl) 
            ? Url.Content("~/images/hero-bg.jpg") 
            : article.ImageUrl;
        ViewData["OgUrl"] = Url.Action("Details", "Blog", new { id = article.Id }, Request.Scheme);

        ViewData["RenderedContent"] = MarkdownHelper.ToHtml(article.Content);
        ViewData["ReadingTime"] = Math.Max(1, article.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length / 200);

        return View(article);
    }
}
