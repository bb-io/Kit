namespace Apps.Kit.Models.Dtos;

public class ProcessedBroadcastHtml
{
    public string Html { get; set; } = string.Empty;

    public string? Subject { get; set; }

    public string? PreviewText { get; set; }

    public string? SourceBroadcastId { get; set; }
}