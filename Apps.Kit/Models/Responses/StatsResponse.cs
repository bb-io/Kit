using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Responses;

public class StatsResponse
{
    [Display("Sent")]
    [JsonProperty("sent")]
    public int? Sent { get; set; }

    [Display("Opened")]
    [JsonProperty("opened")]
    public int? Opened { get; set; }

    [Display("Clicked")]
    [JsonProperty("clicked")]
    public int? Clicked { get; set; }

    [Display("Bounced")]
    [JsonProperty("bounced")]
    public int? Bounced { get; set; }

    [Display("Open rate")]
    [JsonProperty("open_rate")]
    public double? OpenRate { get; set; }

    [Display("Click rate")]
    [JsonProperty("click_rate")]
    public double? ClickRate { get; set; }

    [Display("Last sent")]
    [JsonProperty("last_sent")]
    public DateTime? LastSent { get; set; }

    [Display("Last opened")]
    [JsonProperty("last_opened")]
    public DateTime? LastOpened { get; set; }

    [Display("Last clicked")]
    [JsonProperty("last_clicked")]
    public DateTime? LastClicked { get; set; }

    [Display("Sends since last open")]
    [JsonProperty("sends_since_last_open")]
    public int? SendsSinceLastOpen { get; set; }

    [Display("Sends since last click")]
    [JsonProperty("sends_since_last_click")]
    public int? SendsSinceLastClick { get; set; }
}
