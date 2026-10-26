using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class UpdateSubscriberDto
{
    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("email_address")]
    public string EmailAddress { get; set; } = string.Empty;

    [JsonProperty("fields")]
    public Dictionary<string, string>? Fields { get; set; }
}
