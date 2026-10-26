using Newtonsoft.Json;
using Apps.Kit.Models.Dtos;

namespace Apps.Kit.Models.Responses;

public class GetCustomFieldsResponse
{
    [JsonProperty("custom_fields")]
    public List<CustomFieldDto> CustomFields { get; set; } = [];

    [JsonProperty("pagination")]
    public PaginationDto? Pagination { get; set; }
}
