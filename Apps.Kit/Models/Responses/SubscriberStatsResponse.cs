using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Responses;

public class SubscriberStatsResponse
{
    [Display("Subscriber ID")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Display("Stats")]
    [JsonProperty("stats")]
    public StatsResponse? Stats { get; set; }
}
