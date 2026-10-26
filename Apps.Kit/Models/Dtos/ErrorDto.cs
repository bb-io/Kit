using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class ErrorDto
{
    [JsonProperty("errors")]
    public List<string>? Errors { get; set; }
}
