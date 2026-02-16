using System.ComponentModel.DataAnnotations;

namespace ProfsMarkovHub.Models.Store;

public class StoreItem
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ExternalId { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(500)]
    public string? ImageUrl { get; set; }

    public int Cost { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;
}
