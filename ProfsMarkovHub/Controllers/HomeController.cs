using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;
using ProfsMarkovHub.Models;

namespace ProfsMarkovHub.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        ViewData["OgTitle"] = "ProfsMarkov Hub";
        ViewData["OgDescription"] = "AI tools, stream updates, and latest blog insights.";
        ViewData["OgImage"] = Url.Content("~/images/hero-bg.jpg");
        ViewData["OgUrl"] = Url.Action("Index", "Home", null, Request.Scheme);

        var latest = await _context.Articles
            .Include(a => a.Author)
            .Include(a => a.ArticleTags).ThenInclude(at => at.Tag)
            .OrderByDescending(x => x.PublishedAt)
            .Take(6)
            .ToListAsync();

        return View(latest);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("/Home/StatusCode")]
    public new IActionResult StatusCode(int code)
    {
        if (code == 404)
            return View("~/Views/Shared/NotFound.cshtml");

        return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
