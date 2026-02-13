namespace ProfsMarkovHub.Services.Storage;

public class LocalFallbackStorageService : IAssetStorage
{
    private readonly IWebHostEnvironment _environment;

    public LocalFallbackStorageService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string> SaveAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads");
        Directory.CreateDirectory(uploadFolder);

        var extension = Path.GetExtension(fileName);
        var safeName = $"{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(uploadFolder, safeName);

        await using var output = File.Create(fullPath);
        await stream.CopyToAsync(output, cancellationToken);

        return $"/uploads/{safeName}";
    }
}
