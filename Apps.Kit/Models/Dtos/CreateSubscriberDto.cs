using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class CreateSubscriberDto
{
    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [JsonProperty("email_address")]
    public string EmailAddress { get; set; } = string.Empty;

    [JsonProperty("state")]
    public string? State { get; set; }
}
