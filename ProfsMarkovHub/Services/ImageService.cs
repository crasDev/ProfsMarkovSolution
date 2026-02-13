namespace ProfsMarkovHub.Services;

public class ImageService : IImageService
{
    public Task<string> UploadImageAsync(IFormFile file)
    {
        // Placeholder implementation
        // In a real app, this would upload to Azure Blob Storage or S3
        return Task.FromResult("/images/placeholder.jpg");
    }
}
