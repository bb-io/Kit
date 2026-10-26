using Apps.Kit.Models.Dtos;
using HtmlAgilityPack;

namespace Apps.Kit.Helpers;

public static class BroadcastHtmlHelper
{
    private const string SubjectId = "blackbird-subject";
    private const string PreviewTextId = "blackbird-preview-text";

    public static string PrepareHtml(
        BroadcastDto broadcast,
        string sourceLanguage)
    {
        if (string.IsNullOrWhiteSpace(broadcast.Content))
            throw new ArgumentException("Broadcast has no HTML content.");

        if (string.IsNullOrWhiteSpace(sourceLanguage))
            throw new ArgumentException("Source language is required.");

        var document = new HtmlDocument();
        document.LoadHtml(broadcast.Content);

        var html = document.DocumentNode.SelectSingleNode("//html");

        if (html is null)
        {
            // Kit often returns an HTML fragment.
            // Build a fresh document instead of moving parsed nodes.
            var normalized = new HtmlDocument();
            normalized.LoadHtml(
                "<html><head></head><body></body></html>");

            var normalizedBody = normalized.DocumentNode
                .SelectSingleNode("//body")!;

            normalizedBody.InnerHtml = broadcast.Content;

            document = normalized;
            html = document.DocumentNode.SelectSingleNode("//html")!;
        }

        html.SetAttributeValue("lang", sourceLanguage);

        var head = html.SelectSingleNode("./head");

        if (head is null)
        {
            head = HtmlNode.CreateNode("<head></head>");
            html.PrependChild(head);
        }

        var body = html.SelectSingleNode("./body");

        if (body is null)
        {
            body = HtmlNode.CreateNode("<body></body>");

            foreach (var node in html.ChildNodes.ToList())
            {
                if (node != head)
                    body.AppendChild(node);
            }

            html.AppendChild(body);
        }

        AddMeta(head, "blackbird-ucid", broadcast.Id);
        AddMeta(head, "blackbird-content-name", broadcast.Subject);
        AddMeta(head, "blackbird-system-name", "Kit");
        AddMeta(head, "blackbird-system-ref", "https://kit.com/");

        if (!string.IsNullOrWhiteSpace(broadcast.PublicUrl))
        {
            AddMeta(
                head,
                "blackbird-public-url",
                broadcast.PublicUrl);
        }

        AddTranslatableField(
            body,
            SubjectId,
            broadcast.Subject);

        if (!string.IsNullOrWhiteSpace(broadcast.PreviewText))
        {
            AddTranslatableField(
                body,
                PreviewTextId,
                broadcast.PreviewText);
        }

        return document.DocumentNode.OuterHtml;
    }

    public static ProcessedBroadcastHtml ProcessTranslatedHtml(
        string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            throw new ArgumentException("HTML content is required.");

        var document = new HtmlDocument();
        document.LoadHtml(html);

        // Preserve the original Kit broadcast ID.
        var sourceIdNode = document.DocumentNode.SelectSingleNode(
            "//meta[@name='blackbird-ucid']");

        var sourceBroadcastId = sourceIdNode?
            .GetAttributeValue("content", null);

        // Extract translated fields.
        var subjectNode = document.DocumentNode.SelectSingleNode(
            "//div[@id='blackbird-subject']");

        var previewNode = document.DocumentNode.SelectSingleNode(
            "//div[@id='blackbird-preview-text']");

        var subject = subjectNode is null
            ? null
            : HtmlEntity.DeEntitize(subjectNode.InnerText).Trim();

        var previewText = previewNode is null
            ? null
            : HtmlEntity.DeEntitize(previewNode.InnerText).Trim();

        subjectNode?.Remove();
        previewNode?.Remove();

        // Remove Blackbird metadata before uploading to Kit.
        var metadataNodes = document.DocumentNode.SelectNodes(
            "//meta[starts-with(@name, 'blackbird-')]");

        if (metadataNodes is not null)
        {
            foreach (var node in metadataNodes.ToList())
                node.Remove();
        }

        return new ProcessedBroadcastHtml
        {
            Html = document.DocumentNode.OuterHtml,
            Subject = subject,
            PreviewText = previewText,
            SourceBroadcastId = sourceBroadcastId
        };
    }

    private static void AddMeta(
        HtmlNode head,
        string name,
        string value)
    {
        var existing = head.SelectSingleNode(
            $"./meta[@name='{name}']");

        var meta = existing ?? HtmlNode.CreateNode("<meta>");

        meta.SetAttributeValue("name", name);
        meta.SetAttributeValue("content", value);

        if (existing is null)
            head.AppendChild(meta);
    }

    private static void AddTranslatableField(
        HtmlNode body,
        string id,
        string value)
    {
        var existing = body.SelectSingleNode(
            $"./div[@id='{id}']");

        var field = existing ?? HtmlNode.CreateNode("<div>");

        field.SetAttributeValue("id", id);
        field.SetAttributeValue(
            "data-blackbird-field",
            "true");

        field.InnerHtml = HtmlEntity.Entitize(value);

        if (existing is null)
            body.PrependChild(field);
    }
}
