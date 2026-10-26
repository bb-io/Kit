using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Responses;

public class SubscriberResponse
{
    [Display("Subscriber ID")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Display("First name")]
    [JsonProperty("first_name")]
    public string? FirstName { get; set; }

    [Display("Email address")]
    [JsonProperty("email_address")]
    public string EmailAddress { get; set; } = string.Empty;

    [Display("State")]
    [JsonProperty("state")]
    public string State { get; set; } = string.Empty;

    [Display("Created at")]
    [JsonProperty("created_at")]
    public DateTime? CreatedAt { get; set; }

    [Display("Canceled at")]
    [JsonProperty("canceled_at")]
    public DateTime? CanceledAt { get; set; }

    [Display("Custom fields")]
    [JsonProperty("fields")]
    public Dictionary<string, string>? Fields { get; set; }
}
