using Blackbird.Filters.Transformations;

namespace Apps.Kit.Models.Dtos;

public class InteroperableHtmlContent
{
    // HTML extracted from the uploaded file.
    public string Html { get; set; } = string.Empty;

    // Preserves the original transformation, including
    // bilingual XLIFF data and interoperability metadata.
    public TransformationLoadResult? TransformationResult { get; set; }
}
