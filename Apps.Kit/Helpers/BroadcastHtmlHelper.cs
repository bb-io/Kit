using Apps.Kit.Constants;
using Apps.Kit.Models.Html;
using Apps.Kit.Models.Responses;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;

namespace Apps.Kit.Helpers;

public static class BroadcastHtmlHelper
{
    private const string EmptyHtmlDocument = "<html><head></head><body></body></html>";

    public static string PrepareHtml(BroadcastResponse broadcast, string sourceLanguage)
    {
        if (string.IsNullOrWhiteSpace(broadcast.Content))
            throw new PluginMisconfigurationException("This broadcast has no HTML content.");

        if (string.IsNullOrWhiteSpace(sourceLanguage))
            throw new PluginMisconfigurationException("Source language is required.");

        var document = LoadDocument(broadcast.Content);
        var html = document.DocumentNode.SelectSingleNode("//html")!;
        html.SetAttributeValue("lang", sourceLanguage);

        var head = EnsureHead(html);
        var body = EnsureBody(html, head);

        AddMetadata(head, broadcast);
        AddTranslatableFields(body, broadcast);

        return document.DocumentNode.OuterHtml;
    }

    public static ProcessedBroadcastHtml ProcessTranslatedHtml(string html)
    {
        if (string.IsNullOrWhiteSpace(html))
            throw new PluginMisconfigurationException("The translated file has no HTML content.");

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var sourceBroadcastId = document.DocumentNode
            .SelectSingleNode($"//meta[@name='{BlackbirdMetadata.Ucid}']")
            ?.GetAttributeValue("content", null);

        var subject = ExtractTranslatableField(document, BlackbirdMetadata.SubjectFieldId);
        var previewText = ExtractTranslatableField(document, BlackbirdMetadata.PreviewTextFieldId);

        RemoveBlackbirdMetadata(document);

        return new ProcessedBroadcastHtml
        {
            Html = document.DocumentNode.OuterHtml,
            Subject = subject,
            PreviewText = previewText,
            SourceBroadcastId = sourceBroadcastId
        };
    }

    private static HtmlDocument LoadDocument(string content)
    {
        var document = new HtmlDocument();
        document.LoadHtml(content);

        if (document.DocumentNode.SelectSingleNode("//html") is not null)
            return document;

        var normalized = new HtmlDocument();
        normalized.LoadHtml(EmptyHtmlDocument);
        normalized.DocumentNode.SelectSingleNode("//body")!.InnerHtml = content;

        return normalized;
    }

    private static HtmlNode EnsureHead(HtmlNode html)
    {
        var head = html.SelectSingleNode("./head");
        if (head is not null)
            return head;

        head = HtmlNode.CreateNode("<head></head>");
        html.PrependChild(head);

        return head;
    }

    private static HtmlNode EnsureBody(HtmlNode html, HtmlNode head)
    {
        var body = html.SelectSingleNode("./body");
        if (body is not null)
            return body;

        body = HtmlNode.CreateNode("<body></body>");

        foreach (var node in html.ChildNodes.Where(node => node != head).ToList())
            body.AppendChild(node);

        html.AppendChild(body);

        return body;
    }

    private static void AddMetadata(HtmlNode head, BroadcastResponse broadcast)
    {
        SetMeta(head, BlackbirdMetadata.Ucid, broadcast.Id);
        SetMeta(head, BlackbirdMetadata.ContentName, broadcast.Subject);
        SetMeta(head, BlackbirdMetadata.SystemName, SystemReferences.SystemName);
        SetMeta(head, BlackbirdMetadata.SystemRef, SystemReferences.SystemRef);

        if (!string.IsNullOrWhiteSpace(broadcast.PublicUrl))
            SetMeta(head, BlackbirdMetadata.PublicUrl, broadcast.PublicUrl);
    }

    private static void AddTranslatableFields(HtmlNode body, BroadcastResponse broadcast)
    {
        if (!string.IsNullOrWhiteSpace(broadcast.PreviewText))
            SetTranslatableField(body, BlackbirdMetadata.PreviewTextFieldId, broadcast.PreviewText);

        SetTranslatableField(body, BlackbirdMetadata.SubjectFieldId, broadcast.Subject);
    }

    private static void SetMeta(HtmlNode head, string name, string value)
    {
        var existing = head.SelectSingleNode($"./meta[@name='{name}']");
        var meta = existing ?? HtmlNode.CreateNode("<meta>");

        meta.SetAttributeValue("name", name);
        meta.SetAttributeValue("content", value);

        if (existing is null)
            head.AppendChild(meta);
    }

    private static void SetTranslatableField(HtmlNode body, string id, string value)
    {
        var existing = body.SelectSingleNode($"./div[@id='{id}']");
        var field = existing ?? HtmlNode.CreateNode("<div>");

        field.SetAttributeValue("id", id);
        field.SetAttributeValue(BlackbirdMetadata.FieldAttribute, "true");
        field.InnerHtml = HtmlEntity.Entitize(value);

        if (existing is null)
            body.PrependChild(field);
    }

    private static string? ExtractTranslatableField(HtmlDocument document, string id)
    {
        var node = document.DocumentNode.SelectSingleNode($"//div[@id='{id}']");
        if (node is null)
            return null;

        node.Remove();

        return HtmlEntity.DeEntitize(node.InnerText).Trim();
    }

    private static void RemoveBlackbirdMetadata(HtmlDocument document)
    {
        var metadataNodes = document.DocumentNode
            .SelectNodes($"//meta[starts-with(@name, '{BlackbirdMetadata.MetaPrefix}')]")
            ?.ToList() ?? [];

        foreach (var node in metadataNodes)
            node.Remove();
    }
}
