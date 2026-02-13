namespace ProfsMarkovHub.Services.Store;

public class StreamElementsOptions
{
    public const string SectionName = "StreamElements";

    public string ChannelId { get; set; } = "replace-with-channel-id";
    public string JwtToken { get; set; } = "replace-with-jwt-token";
    public string BaseUrl { get; set; } = "https://api.streamelements.com";
}
