using ProfsMarkovHub.Models.Store;

namespace ProfsMarkovHub.ViewModels;

public class StoreViewModel
{
    public IReadOnlyCollection<StoreItem> Items { get; init; } = Array.Empty<StoreItem>();
    public bool IsFallback { get; init; }
    public string? Message { get; init; }
}
