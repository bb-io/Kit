using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Kit.Models.Responses;

public class EmailTemplateResponse
{
    [Display("Email template ID")]
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [Display("Name")]
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
