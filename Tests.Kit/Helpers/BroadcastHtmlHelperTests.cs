using Apps.Kit.Helpers;
using Apps.Kit.Models.Responses;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Kit.Helpers;

[TestClass]
public class BroadcastHtmlHelperTests
{
    [TestMethod]
    public void PrepareHtml_AddsBlackbirdMetadata()
    {
        var broadcast = new BroadcastResponse
        {
            ContentId = "12345",
            Subject = "October Newsletter",
            PublicUrl = "https://example.com/newsletter",
            Content = "<html><head></head><body><p>Hello!</p></body></html>"
        };

        var result = BroadcastHtmlHelper.PrepareHtml(
            broadcast, "en");

        var document = new HtmlDocument();
        document.LoadHtml(result);

        Assert.AreEqual(
            "en",
            document.DocumentNode.SelectSingleNode("//html")
                ?.GetAttributeValue("lang", ""));

        Assert.AreEqual(
            "12345",
            GetMeta(document, "blackbird-ucid"));

        Assert.AreEqual(
            "October Newsletter",
            GetMeta(document, "blackbird-content-name"));

        Assert.AreEqual(
            "Kit",
            GetMeta(document, "blackbird-system-name"));

        Assert.AreEqual(
            "https://example.com/newsletter",
            GetMeta(document, "blackbird-public-url"));

        Assert.IsTrue(result.Contains("Hello!"));
    }

    [TestMethod]
    public void PrepareHtml_HandlesHtmlFragments()
    {
        var broadcast = new BroadcastResponse
        {
            ContentId = "67890",
            Subject = "Test Broadcast",
            Content = "<h1>Welcome</h1><p>Thanks for subscribing.</p>"
        };

        var result = BroadcastHtmlHelper.PrepareHtml(
            broadcast, "en");

        var document = new HtmlDocument();
        document.LoadHtml(result);

        Assert.IsNotNull(
            document.DocumentNode.SelectSingleNode("//html"));

        Assert.IsNotNull(
            document.DocumentNode.SelectSingleNode("//head"));

        Assert.AreEqual(
            "67890",
            GetMeta(document, "blackbird-ucid"));

        Assert.IsTrue(result.Contains("Welcome"));
        Assert.IsTrue(result.Contains("Thanks for subscribing."));
    }

    [TestMethod]
    public void PrepareHtml_DoesNotDuplicateMetadata()
    {
        var broadcast = new BroadcastResponse
        {
            ContentId = "99999",
            Subject = "Updated Newsletter",
            Content = """
                <html>
                <head>
                    <meta name="blackbird-ucid" content="old-id">
                </head>
                <body><p>Content</p></body>
                </html>
                """
        };

        var result = BroadcastHtmlHelper.PrepareHtml(
            broadcast, "en");

        var document = new HtmlDocument();
        document.LoadHtml(result);

        var tags = document.DocumentNode.SelectNodes(
            "//meta[@name='blackbird-ucid']");

        Assert.IsNotNull(tags);
        Assert.AreEqual(1, tags.Count);
        Assert.AreEqual(
            "99999",
            tags[0].GetAttributeValue("content", ""));
    }

    private static string? GetMeta(
        HtmlDocument document,
        string name)
    {
        return document.DocumentNode
            .SelectSingleNode($"//meta[@name='{name}']")
            ?.GetAttributeValue("content", "");
    }
    [TestMethod]
    public void PrepareHtml_IncludesTranslatableSubjectAndPreview()
    {
        // Arrange
        var broadcast = new BroadcastResponse
        {
            ContentId = "12345",
            Subject = "October Newsletter",
            PreviewText = "Check out our latest updates",
            Content = "<p>Hello subscribers!</p>"
        };

        // Act
        var html = BroadcastHtmlHelper.PrepareHtml(
            broadcast,
            "en-US");

        // Assert
        var document = new HtmlAgilityPack.HtmlDocument();
        document.LoadHtml(html);

        var subject = document.DocumentNode.SelectSingleNode(
            "//div[@id='blackbird-subject']");

        var preview = document.DocumentNode.SelectSingleNode(
            "//div[@id='blackbird-preview-text']");

        Assert.IsNotNull(subject);
        Assert.IsNotNull(preview);

        Assert.AreEqual(
            "October Newsletter",
            subject.InnerText);

        Assert.AreEqual(
            "Check out our latest updates",
            preview.InnerText);

        // Original content must remain intact.
        Assert.IsTrue(html.Contains("Hello subscribers!"));
    }


    [TestMethod]
    public void ProcessTranslatedHtml_ExtractsFieldsAndCleansHtml()
    {
        // Arrange: simulate a translated German broadcast.
        var translatedHtml = """
        <html lang="de-DE">
        <head>
            <meta name="blackbird-ucid" content="12345">
            <meta name="blackbird-system-name" content="Kit">
        </head>
        <body>
            <div id="blackbird-subject"
                 data-blackbird-field="true">Oktober-Newsletter</div>
            <div id="blackbird-preview-text"
                 data-blackbird-field="true">Unsere neuesten Updates</div>
            <p>Hallo Abonnenten!</p>
        </body>
        </html>
        """;

        // Act
        var result = BroadcastHtmlHelper.ProcessTranslatedHtml(
            translatedHtml);

        // Assert: translated fields are extracted.
        Assert.AreEqual(
            "Oktober-Newsletter",
            result.Subject);

        Assert.AreEqual(
            "Unsere neuesten Updates",
            result.PreviewText);

        // The translated body is preserved.
        StringAssert.Contains(
            result.Html,
            "Hallo Abonnenten!");

        // Temporary Blackbird elements are removed.
        Assert.IsFalse(result.Html.Contains("blackbird-subject"));
        Assert.IsFalse(result.Html.Contains("blackbird-preview-text"));
        Assert.IsFalse(result.Html.Contains("blackbird-ucid"));
        Assert.IsFalse(result.Html.Contains("blackbird-system-name"));
        Assert.AreEqual("12345", result.SourceBroadcastId);
    }


    [TestMethod]
    public void BroadcastHtml_RoundTripPreservesContentAndSourceId()
    {
        // Arrange: original English broadcast.
        var broadcast = new BroadcastResponse
        {
            ContentId = "12345",
            Subject = "October Newsletter",
            PreviewText = "Latest updates",
            Content = "<p>Hello subscribers!</p>"
        };

        // Export for translation.
        var exportedHtml = BroadcastHtmlHelper.PrepareHtml(
            broadcast, "en-US");

        // Simulate translated content.
        var translatedHtml = exportedHtml
            .Replace("October Newsletter", "Oktober-Newsletter")
            .Replace("Latest updates", "Neueste Nachrichten")
            .Replace("Hello subscribers!", "Hallo Abonnenten!");

        // Process the translated content.
        var result = BroadcastHtmlHelper.ProcessTranslatedHtml(
            translatedHtml);

        // Verify translated fields.
        Assert.AreEqual("Oktober-Newsletter", result.Subject);
        Assert.AreEqual("Neueste Nachrichten", result.PreviewText);

        // Verify source relationship.
        Assert.AreEqual("12345", result.SourceBroadcastId);

        // Verify translated body.
        StringAssert.Contains(result.Html, "Hallo Abonnenten!");

        // Verify temporary elements were removed.
        Assert.IsFalse(result.Html.Contains("blackbird-subject"));
        Assert.IsFalse(result.Html.Contains("blackbird-preview-text"));
        Assert.IsFalse(result.Html.Contains("blackbird-ucid"));
    }
}
