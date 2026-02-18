using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ProfsMarkovHub.Models;

public enum ArticleStatus
{
    Draft = 0,
    Published = 1,
    Scheduled = 2
}

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

    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public DateTime? PublishedAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public string? AuthorId { get; set; }
    public IdentityUser? Author { get; set; }

    public string? ImageUrl { get; set; }

    [MaxLength(200)]
    public string Excerpt { get; set; } = string.Empty;

    // SEO overrides (optional)
    [MaxLength(200)]
    public string? OgTitle { get; set; }

    [MaxLength(300)]
    public string? OgDescription { get; set; }

    [MaxLength(500)]
    [Url]
    public string? OgImageUrl { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; } = false;

    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}
