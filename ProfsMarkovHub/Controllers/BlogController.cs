using Microsoft.AspNetCore.Mvc;
using ProfsMarkovHub.Helpers;
using System.Linq;
using ProfsMarkovHub.Models;

namespace ProfsMarkovHub.Controllers
{
    public class BlogController : Controller
    {
        public async Task<IActionResult> Details(string slug)
        {
            var article = await _context.Articles
                .Include(a => a.Tags)
                .FirstOrDefaultAsync(a => a.Slug == slug);

            if (article == null)
            {
                return NotFound();
            }

            article.Content = MarkdownHelper.ToHtml(article.Content);
            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Title,Content,Tags")] Article article)
        {
            if (ModelState.IsValid)
            {
                article.Slug = GenerateSlug(article.Title);
                article.Excerpt = GenerateExcerpt(article.Content);
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(article);
        }

        private string GenerateSlug(string title) => title.ToLower().Replace(" ", "-").Replace(".", "");

        private string GenerateExcerpt(string content)
        {
            var plainText = MarkdownHelper.ToHtml(content);
            return new string(plainText.Take(200).ToArray());
        }
    }
}