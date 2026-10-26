using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class EmailTemplateDto
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
}
