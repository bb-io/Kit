using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class PaginationDto
{
    [JsonProperty("has_previous_page")]
    public bool HasPreviousPage { get; set; }

    [JsonProperty("has_next_page")]
    public bool HasNextPage { get; set; }

    [JsonProperty("start_cursor")]
    public string? StartCursor { get; set; }

    [JsonProperty("end_cursor")]
    public string? EndCursor { get; set; }

    [JsonProperty("per_page")]
    public int PerPage { get; set; }
}
