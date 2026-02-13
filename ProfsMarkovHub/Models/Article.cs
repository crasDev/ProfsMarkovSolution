using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProfsMarkovHub.Models;

public class Article
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty; // Markdown/HTML

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    public string? AuthorId { get; set; }
    public IdentityUser? Author { get; set; }

    public string? ImageUrl { get; set; }

    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}
