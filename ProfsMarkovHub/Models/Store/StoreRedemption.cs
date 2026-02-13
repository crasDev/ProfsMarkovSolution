using System.ComponentModel.DataAnnotations;

namespace ProfsMarkovHub.Models.Store;

public class StoreRedemption
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string ExternalId { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string StoreItemExternalId { get; set; } = string.Empty;

    [Required]
    [MaxLength(160)]
    public string StoreItemName { get; set; } = string.Empty;

    [Required]
    [MaxLength(120)]
    public string ViewerName { get; set; } = string.Empty;

    public int Cost { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = "pending";

    public DateTime RedeemedAt { get; set; } = DateTime.UtcNow;
}
