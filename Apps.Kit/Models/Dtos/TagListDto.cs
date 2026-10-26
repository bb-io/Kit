using Newtonsoft.Json;

namespace Apps.Kit.Models.Dtos;

public class TagListDto : PaginatedDto<TagDto>
{
    [JsonProperty("tags")]
    public List<TagDto> Tags { get; set; } = [];

    public override List<TagDto> Items => Tags;
}
