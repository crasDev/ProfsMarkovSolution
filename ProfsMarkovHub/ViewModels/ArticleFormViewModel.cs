using System.ComponentModel.DataAnnotations;
using ProfsMarkovHub.Models;

namespace ProfsMarkovHub.ViewModels;

public class ArticleFormViewModel
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Slug { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Excerpt { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;

    public DateTime? PublishedAt { get; set; }

    public DateTime? ScheduledAt { get; set; }

    public string? AuthorId { get; set; }

    // SEO overrides
    [MaxLength(200)]
    public string? OgTitle { get; set; }

    [MaxLength(300)]
    public string? OgDescription { get; set; }

    [MaxLength(500)]
    public string? OgImageUrl { get; set; }

    /// <summary>Comma-separated tag names (e.g. "WoW, Horror, Indie")</summary>
    public string TagsCsv { get; set; } = string.Empty;
}
