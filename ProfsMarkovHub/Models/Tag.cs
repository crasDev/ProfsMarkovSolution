using System.ComponentModel.DataAnnotations;

namespace ProfsMarkovHub.Models;

public class Tag
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string Slug { get; set; } = string.Empty;

    public ICollection<ArticleTag> ArticleTags { get; set; } = new List<ArticleTag>();
}
