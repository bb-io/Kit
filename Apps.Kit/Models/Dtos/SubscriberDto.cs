using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class SubscriberDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("email_address")]
    public string EmailAddress { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonProperty("fields")]
    public Dictionary<string, string>? Fields { get; set; }

    [JsonProperty("canceled_at")]
    public DateTime? CanceledAt { get; set; }
}
