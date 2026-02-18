using System.Text;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using ProfsMarkovHub.Data;
using ProfsMarkovHub.Models;

namespace ProfsMarkovHub.Services;

public interface ISlugService
{
    string GenerateSlug(string title);
    Task<bool> IsUniqueAsync(string slug, int? excludingArticleId = null);
}

public class SlugService : ISlugService
{
    private readonly ApplicationDbContext _context;
    public SlugService(ApplicationDbContext context) { _context = context; }

    public string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return string.Empty;
        var slug = title.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, "[\\s_]+", "-");
        slug = Regex.Replace(slug, "[^a-z0-9-]", "");
        slug = Regex.Replace(slug, "-+", "-");
        slug = slug.Trim('-');
        return slug;
    }

    public async Task<bool> IsUniqueAsync(string slug, int? excludingArticleId = null)
    {
        if (string.IsNullOrWhiteSpace(slug)) return false;
        var query = _context.Articles.AsQueryable();
        if (excludingArticleId.HasValue)
            query = query.Where(a => a.Id != excludingArticleId.Value);
        return !await query.AnyAsync(a => a.Slug == slug && !a.IsDeleted);
    }
}
