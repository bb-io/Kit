using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public abstract class PaginatedDto<T>
{
    [JsonProperty("pagination")]
    public PaginationDto? Pagination { get; set; }

    [JsonIgnore]
    public abstract List<T> Items { get; }
}
