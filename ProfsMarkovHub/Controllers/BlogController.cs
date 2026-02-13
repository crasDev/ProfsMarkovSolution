using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;

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

        return View(await _context.Articles.Include(a => a.Author).OrderByDescending(a => a.PublishedAt).ToListAsync());
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var article = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags)
                .ThenInclude(at => at.Tag)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (article == null)
        {
            return NotFound();
        }

        return View(article);
    }
}
