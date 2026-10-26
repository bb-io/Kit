using Blackbird.Filters.Transformations;

namespace Apps.Kit.Models.Html;

public class InteroperableHtmlContent
{
    public string Html { get; set; } = string.Empty;

    public Transformation? Transformation { get; set; }

    public bool WasBilingual { get; set; }
}
