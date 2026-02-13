namespace ProfsMarkovHub.Services.Storage;

public interface IAssetStorage
{
    Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default);
}
