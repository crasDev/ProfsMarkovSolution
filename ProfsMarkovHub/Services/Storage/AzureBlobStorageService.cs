using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Options;

namespace ProfsMarkovHub.Services.Storage;

public class AzureBlobStorageOptions
{
    public const string SectionName = "AzureBlobStorage";

    public string ConnectionString { get; set; } = "UseDevelopmentStorage=true";
    public string ContainerName { get; set; } = "assets";
    public string PublicBaseUrl { get; set; } = string.Empty;
}

public class AzureBlobStorageService : IAssetStorage
{
    private readonly BlobContainerClient _containerClient;
    private readonly AzureBlobStorageOptions _options;

    public AzureBlobStorageService(IOptions<AzureBlobStorageOptions> options)
    {
        _options = options.Value;
        var serviceClient = new BlobServiceClient(_options.ConnectionString);
        _containerClient = serviceClient.GetBlobContainerClient(_options.ContainerName);
        _containerClient.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(fileName);
        var blobName = $"articles/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid():N}{extension}";

        var blob = _containerClient.GetBlobClient(blobName);
        await blob.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);

        if (!string.IsNullOrWhiteSpace(_options.PublicBaseUrl))
        {
            return $"{_options.PublicBaseUrl.TrimEnd('/')}/{blobName}";
        }

        return blob.Uri.ToString();
    }
}
