using Apps.Kit.Actions;
using Apps.Kit.Constants;
using Apps.Kit.Helpers;
using Apps.Kit.Models.Requests;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Filters.Transformations;
using Newtonsoft.Json;
using Tests.Kit.Base;

namespace Tests.Kit;

[TestClass]
public class BroadcastActionsTests : TestBase
{
    private const string SourceBroadcastId = "26340985";
    private const string LocalizedBroadcastId = "26343184";
    private const string XliffFileName = "kit-broadcast-26340985.html.xlf";

    [TestMethod]
    public async Task SearchBroadcasts_ShouldReturnBroadcasts()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);

        var result = await actions.SearchBroadcasts(new SearchBroadcastsRequest());

        Assert.IsTrue(result.Broadcasts.Count > 0);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task GetBroadcast_ValidId_ShouldReturnBroadcast()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);
        var request = new BroadcastRequest { BroadcastId = "26025023" };

        var result = await actions.GetBroadcast(request);

        Assert.AreEqual(request.BroadcastId, result.ContentId);
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task CreateBroadcast_ShouldCreateDraft()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);
        var request = new CreateBroadcastRequest
        {
            Subject = "Blackbird Integration Test",
            PreviewText = "Testing the Kit API",
            Description = "Created during Blackbird Dev Lab",
            Content = new FileReference { Name = "test-broadcast.html", ContentType = "text/html" }
        };

        var result = await actions.CreateBroadcast(request);

        Assert.IsFalse(string.IsNullOrEmpty(result.ContentId));
        Assert.AreEqual(BroadcastStatuses.Draft, result.Status);
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task DownloadBroadcast_ShouldReturnInteroperableHtml()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);
        var request = new DownloadBroadcastRequest { ContentId = SourceBroadcastId, SourceLanguage = "en-US" };

        var result = await actions.DownloadBroadcast(request);

        var html = FileManager.ReadOutputAsString(result.Content);
        Assert.AreEqual("text/html", result.Content.ContentType);
        StringAssert.Contains(html, BlackbirdMetadata.Ucid);
        StringAssert.Contains(html, BlackbirdMetadata.SystemName);
        StringAssert.Contains(html, "lang=\"en-US\"");
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task UploadBroadcast_HtmlWithoutBroadcastId_ShouldCreateLocalizedDraft()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);
        var translatedFile = await PrepareTranslatedHtml(actions, "broadcast-de-DE.html", "Dies ist eine deutsche Version.");

        var result = await actions.UploadBroadcast(new UploadBroadcastRequest
        {
            Content = translatedFile,
            Locale = "de-DE"
        });

        Assert.AreNotEqual(SourceBroadcastId, result.Broadcast.ContentId);
        Assert.AreEqual(BroadcastStatuses.Draft, result.Broadcast.Status);
        Assert.AreEqual("Integrationstest-Newsletter", result.Broadcast.Subject);
        StringAssert.EndsWith(result.Broadcast.Description, "[de-DE]");
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task UploadBroadcast_HtmlWithBroadcastId_ShouldUpdateLocalizedDraft()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);
        var existing = await actions.GetBroadcast(new BroadcastRequest { BroadcastId = LocalizedBroadcastId });
        var translatedFile = await PrepareTranslatedHtml(actions, "broadcast-update-de.html", "Dies ist die aktualisierte deutsche Version.");

        var result = await actions.UploadBroadcast(new UploadBroadcastRequest
        {
            Content = translatedFile,
            Locale = "de",
            ContentId = LocalizedBroadcastId
        });

        var updated = await actions.GetBroadcast(new BroadcastRequest { BroadcastId = LocalizedBroadcastId });
        Assert.AreEqual(LocalizedBroadcastId, result.Broadcast.ContentId);
        Assert.AreEqual(existing.Description, updated.Description);
        StringAssert.Contains(updated.Content, "Dies ist die aktualisierte deutsche Version.");
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task UploadBroadcast_XliffWithBroadcastId_ShouldStampTargetReference()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);

        var result = await actions.UploadBroadcast(new UploadBroadcastRequest
        {
            Content = new FileReference { Name = XliffFileName, ContentType = "application/xliff+xml" },
            Locale = "de",
            ContentId = LocalizedBroadcastId
        });

        FileManager.CopyOutputToInput(result.Content);
        var returnedFile = await FileManager.DownloadAsync(result.Content);
        var transformation = Transformation.Load(returnedFile, result.Content.Name, result.Content.ContentType);

        Assert.IsTrue(transformation.Success, transformation.Error);
        Assert.AreEqual(LocalizedBroadcastId, transformation.Value.TargetSystemReference.ContentId);
        Assert.AreEqual(SystemReferences.SystemName, transformation.Value.TargetSystemReference.SystemName);
        Assert.AreEqual("de", transformation.Value.TargetLanguage);
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task UploadBroadcast_SourceBroadcastId_ShouldThrow()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);
        var translatedFile = await PrepareTranslatedHtml(actions, "broadcast-source-de.html", "Dies ist eine deutsche Version.");

        await Assert.ThrowsExceptionAsync<Blackbird.Applications.Sdk.Common.Exceptions.PluginMisconfigurationException>(() =>
            actions.UploadBroadcast(new UploadBroadcastRequest
            {
                Content = translatedFile,
                Locale = "de",
                ContentId = SourceBroadcastId
            }));
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task UpdateBroadcast_ShouldUpdateDraft()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);
        var request = new UpdateBroadcastRequest
        {
            BroadcastId = "26301241",
            Subject = "Updated broadcast test",
            PreviewText = "Testing the Kit update action",
            Description = "Updated through Blackbird",
            Content = new FileReference { Name = "test-broadcast.html" }
        };

        var result = await actions.UpdateBroadcast(request);

        Assert.AreEqual(request.BroadcastId, result.ContentId);
        Assert.AreEqual(request.Subject, result.Subject);
        Assert.IsFalse(result.Public);
    }

    private async Task<FileReference> PrepareTranslatedHtml(BroadcastActions actions, string fileName, string translatedSentence)
    {
        var source = await actions.GetBroadcast(new BroadcastRequest { BroadcastId = SourceBroadcastId });
        var html = BroadcastHtmlHelper.PrepareHtml(source, "en-US")
            .Replace("Integration Test Broadcast", "Integrationstest-Newsletter")
            .Replace("This is an English version.", translatedSentence);

        return FileManager.SaveInputFile(fileName, html, "text/html");
    }
}
