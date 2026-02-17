using System.ComponentModel.DataAnnotations;

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

    public DateTime PublishedAt { get; set; } = DateTime.UtcNow;

    public string? AuthorId { get; set; }

    /// <summary>Comma-separated tag names (e.g. "WoW, Horror, Indie")</summary>
    public string TagsCsv { get; set; } = string.Empty;
}
