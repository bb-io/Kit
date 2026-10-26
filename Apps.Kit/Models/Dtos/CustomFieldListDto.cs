using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class CustomFieldListDto : PaginatedDto<CustomFieldDto>
{
    [JsonProperty("custom_fields")]
    public List<CustomFieldDto> CustomFields { get; set; } = [];

    public override List<CustomFieldDto> Items => CustomFields;
}
