using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using ProfsMarkovHub.Models.Store;

namespace ProfsMarkovHub.Services.Store;

public class StreamElementsService : IStreamElementsService
{
    private readonly HttpClient _httpClient;
    private readonly StreamElementsOptions _options;
    private readonly ILogger<StreamElementsService> _logger;

    public StreamElementsService(HttpClient httpClient, IOptions<StreamElementsOptions> options, ILogger<StreamElementsService> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;

        if (!string.IsNullOrWhiteSpace(_options.JwtToken) && !_options.JwtToken.StartsWith("replace-with"))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.JwtToken);
        }
    }

    public async Task<IReadOnlyCollection<StoreItem>> GetStoreItemsAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.ChannelId) || _options.ChannelId.StartsWith("replace-with"))
        {
            throw new InvalidOperationException("StreamElements is not configured. Please set StreamElements:ChannelId and StreamElements:JwtToken.");
        }

        var endpoint = $"/kappa/v2/store/{_options.ChannelId}/items";
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var dto = await JsonSerializer.DeserializeAsync<List<StreamElementsStoreItemDto>>(stream, cancellationToken: cancellationToken)
            ?? new List<StreamElementsStoreItemDto>();

        return dto.Select(x => new StoreItem
        {
            ExternalId = x.Id ?? Guid.NewGuid().ToString("N"),
            Name = x.Name ?? "Unknown Reward",
            Description = x.Description,
            Cost = x.Cost,
            ImageUrl = x.ImageUrl,
            IsActive = x.Enabled,
            LastSyncedAt = DateTime.UtcNow
        }).ToList();
    }

    private sealed class StreamElementsStoreItemDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Cost { get; set; }
        public string? ImageUrl { get; set; }
        public bool Enabled { get; set; } = true;
    }
}
