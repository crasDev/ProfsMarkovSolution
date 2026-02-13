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

    // GET: Blog
    public async Task<IActionResult> Index()
    {
        return View(await _context.Articles.Include(a => a.Author).ToListAsync());
    }

    // GET: Blog/Details/5
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
