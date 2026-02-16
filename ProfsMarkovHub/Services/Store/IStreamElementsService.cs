using ProfsMarkovHub.Models.Store;

namespace ProfsMarkovHub.Services.Store;

public interface IStreamElementsService
{
    Task<IReadOnlyCollection<StoreItem>> GetStoreItemsAsync(CancellationToken cancellationToken = default);
}
