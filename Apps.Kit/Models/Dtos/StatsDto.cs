using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class StatsDto
{
    [JsonProperty("sent")]
    public int? Sent { get; set; }

    [JsonProperty("opened")]
    public int? Opened { get; set; }

    [JsonProperty("clicked")]
    public int? Clicked { get; set; }

    [JsonProperty("bounced")]
    public int? Bounced { get; set; }

    [JsonProperty("open_rate")]
    public double? OpenRate { get; set; }

    [JsonProperty("click_rate")]
    public double? ClickRate { get; set; }

    [JsonProperty("last_sent")]
    public DateTime? LastSent { get; set; }

    [JsonProperty("last_opened")]
    public DateTime? LastOpened { get; set; }

    [JsonProperty("last_clicked")]
    public DateTime? LastClicked { get; set; }

    [JsonProperty("sends_since_last_open")]
    public int? SendsSinceLastOpen { get; set; }

    [JsonProperty("sends_since_last_click")]
    public int? SendsSinceLastClick { get; set; }
}
