using Apps.Kit.Actions;
using Apps.Kit.Models.Requests;
using Newtonsoft.Json;
using Tests.Kit.Base;
using Blackbird.Applications.Sdk.Common.Files;
using Apps.Kit.Helpers;
using Blackbird.Filters.Transformations;
using System.Text;

namespace Tests.Kit;

[TestClass]
public class BroadcastActionsTests : TestBase
{
    [TestMethod]
    public async Task GetBroadcast_ValidId_ShouldReturnBroadcast()
    {
        var broadcastActions = new BroadcastActions(InvocationContext, null!);
        var request = new BroadcastRequest
        {
            BroadcastId = "26025023"
        };

        var result = await broadcastActions.GetBroadcast(request);

        Assert.IsNotNull(result);
        Assert.AreEqual(request.BroadcastId, result.Id);
        Console.WriteLine($"EmailTemplate: {result.EmailTemplate?.Id}");
        Console.WriteLine($"EmailTemplate: {result.EmailTemplate?.Name}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    public async Task SearchBroadcasts_ShouldReturnBroadcasts()
    {
        var broadcastActions = new BroadcastActions(InvocationContext, null!);

        var request = new SearchBroadcastsRequest();

        var result = await broadcastActions.SearchBroadcasts(request);

        Assert.IsNotNull(result);
        Assert.IsTrue(result.Count > 0);

        Console.WriteLine($"Broadcasts returned: {result.Count}");
        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task CreateBroadcast_ShouldCreateDraft()
    {
        var broadcastActions = new BroadcastActions(InvocationContext, FileManager);

        var request = new CreateBroadcastRequest
        {
            Subject = "Blackbird Integration Test",
            PreviewText = "Testing the Kit API",
            Description = "Created during Blackbird Dev Lab",
            Content = new FileReference
            {
                Name = "test-broadcast.html",
                ContentType = "text/html"
            }
        };

        var result = await broadcastActions.CreateBroadcast(request);

        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrEmpty(result.Id));
        Assert.AreEqual("draft", result.Status);

        Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
    }

    [TestCategory("Manual")]
    [TestMethod]
    public async Task DownloadBroadcast_ShouldReturnHtmlFile()
    {
        var broadcastActions = new BroadcastActions(
            InvocationContext,
            FileManager);

        var request = new DownloadBroadcastRequest
        {
            BroadcastId = "23450086",
            SourceLanguage = "en"
        };

        var result = await broadcastActions.DownloadBroadcast(request);

        Assert.IsNotNull(result.Content);
        Assert.AreEqual("text/html", result.Content.ContentType);

        var html = FileManager.ReadOutputAsString(result.Content);

        Assert.IsFalse(string.IsNullOrWhiteSpace(html));
        Assert.IsTrue(html.Contains("blackbird-ucid"));
        Assert.IsTrue(html.Contains("blackbird-system-name"));
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task UpdateBroadcast_ShouldUpdateDraft()
    {
        var actions = new BroadcastActions(InvocationContext, FileManager);

        var input = new UpdateBroadcastRequest
        {
            BroadcastId = "26301241",
            Subject = "Updated broadcast test",
            PreviewText = "Testing the Kit update action",
            Description = "Updated through Blackbird",
            Content = new FileReference
            {
                Name = "test-broadcast.html"
            }
        };

        var result = await actions.UpdateBroadcast(input);

        Assert.AreEqual(input.BroadcastId, result.Id);
        Assert.AreEqual(input.Subject, result.Subject);
        Assert.AreEqual(false, result.Public);

        Console.WriteLine($"Updated broadcast: {result.Id}");
        Console.WriteLine($"Status: {result.Status}");
    }


    [TestMethod]
    [TestCategory("Manual")]
    public async Task CreateLocalizedBroadcast_ShouldCreateGermanDraft()
    {
        // Arrange
        var actions = new BroadcastActions(
            InvocationContext,
            FileManager);

        const string originalBroadcastId = "26340985";
        const string targetLanguage = "de-DE";
        const string germanSentence = "Dies ist eine deutsche Version.";

        // 1. Retrieve the original English broadcast.
        var original = await actions.GetBroadcast(
            new BroadcastRequest
            {
                BroadcastId = originalBroadcastId
            });

        // 2. Export the original HTML with Blackbird metadata.
        var html = BroadcastHtmlHelper.PrepareHtml(
            original,
            "en-US");

        const string englishSentence = "This is an English version.";

        var originalCount = (original.Content ?? string.Empty)
            .Split(englishSentence, StringSplitOptions.None).Length - 1;

        var preparedCount = html
            .Split(englishSentence, StringSplitOptions.None).Length - 1;

        Console.WriteLine($"Original Kit content occurrences: {originalCount}");
        Console.WriteLine($"Prepared HTML occurrences: {preparedCount}");

        // 3. Simulate German translation.
        html = html
            .Replace(
                "Integration Test Broadcast",
                "Integrationstest-Newsletter")
            .Replace(
                "This is an English version.",
                germanSentence);

        // 4. Process the translated HTML before uploading.
        var processed = BroadcastHtmlHelper.ProcessTranslatedHtml(html);

        var occurrencesBeforeUpload = processed.Html.Split(
            germanSentence,
            StringSplitOptions.None).Length - 1;

        Console.WriteLine(
            $"German sentence occurrences before upload: {occurrencesBeforeUpload}");

        Console.WriteLine(
            $"Processed HTML length: {processed.Html.Length}");

        // Verify our helper produces the correct content.
        Assert.AreEqual(
            1,
            occurrencesBeforeUpload,
            "The HTML helper duplicated content before upload.");

        Assert.AreEqual(
            originalBroadcastId,
            processed.SourceBroadcastId);

        Assert.AreEqual(
            "Integrationstest-Newsletter",
            processed.Subject);

        // 5. Save the translated HTML to the test input directory.
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var projectDirectory = Directory
            .GetParent(baseDirectory)?
            .Parent?
            .Parent?
            .Parent?
            .FullName
            ?? throw new Exception(
                "Could not locate the test project directory.");

        var inputDirectory = Path.Combine(
            projectDirectory,
            "TestFiles",
            "Input");

        Directory.CreateDirectory(inputDirectory);

        const string fileName = "broadcast-de-DE.html";

        var filePath = Path.Combine(inputDirectory, fileName);

        await File.WriteAllTextAsync(
            filePath,
            html,
            System.Text.Encoding.UTF8);

        var translatedFile = new FileReference
        {
            Name = fileName,
            ContentType = "text/html"
        };

        // 6. Create the localized draft in Kit.
        var result = await actions.CreateLocalizedBroadcast(
            new CreateLocalizedBroadcastRequest
            {
                Content = translatedFile,
                TargetLanguage = targetLanguage
            });

        // 7. Validate the created broadcast.
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Broadcast.Id));
        Assert.AreNotEqual(original.Id, result.Broadcast.Id);

        Assert.AreEqual(
            "Integrationstest-Newsletter",
            result.Broadcast.Subject);

        Assert.AreEqual("[de-DE]", result.Broadcast.Description);
        Assert.AreEqual("draft", result.Broadcast.Status);

        // 8. Retrieve the newly created broadcast from Kit.
        var created = await actions.GetBroadcast(
            new BroadcastRequest
            {
                BroadcastId = result.Broadcast.Id
            });

        var occurrencesAfterUpload =
            (created.Content ?? string.Empty).Split(
                germanSentence,
                StringSplitOptions.None).Length - 1;

        Console.WriteLine(
            $"German sentence occurrences after upload: {occurrencesAfterUpload}");

        Console.WriteLine(
            $"Created broadcast ID: {created.Id}");

        Console.WriteLine(
            $"Created broadcast status: {created.Status}");

        Console.WriteLine(
            $"Created HTML length: {created.Content?.Length ?? 0}");

        // Verify translated content exists.
        StringAssert.Contains(
            created.Content ?? string.Empty,
            germanSentence);

        // 9. Report the result without failing on Kit-side duplication.
        if (occurrencesAfterUpload > occurrencesBeforeUpload)
        {
            Console.WriteLine(
                "WARNING: Content duplication occurred after submission to Kit.");
        }
        else
        {
            Console.WriteLine(
                "No additional duplication detected in the Kit API response.");
        }
    }

    [TestMethod]
    [TestCategory("Manual")]
    public async Task ExportBroadcastHtml_ShouldSaveLocally()
    {
        var actions = new BroadcastActions(
            InvocationContext,
            FileManager);

        const string broadcastId = "26340985";

        // Retrieve the original broadcast from Kit.
        var broadcast = await actions.GetBroadcast(
            new BroadcastRequest
            {
                BroadcastId = broadcastId
            });

        // Generate the same interoperable HTML used by
        // the Download broadcast action.
        var html = BroadcastHtmlHelper.PrepareHtml(
            broadcast,
            "en-US");

        // Save directly to Downloads.
        var downloadsFolder = Path.Combine(
            Environment.GetFolderPath(
                Environment.SpecialFolder.UserProfile),
            "Downloads");

        var outputPath = Path.Combine(
            downloadsFolder,
            $"kit-broadcast-{broadcastId}.html");

        await File.WriteAllTextAsync(
            outputPath,
            html,
            new System.Text.UTF8Encoding(false));

        Console.WriteLine($"Exported HTML: {outputPath}");
        Console.WriteLine($"File size: {new FileInfo(outputPath).Length} bytes");

        Assert.IsTrue(File.Exists(outputPath));
        StringAssert.Contains(html, "blackbird-ucid");
        StringAssert.Contains(html, "blackbird-system-name");
        StringAssert.Contains(html, "lang=\"en-US\"");
    }


    [TestMethod]
    [TestCategory("Manual")]
    public async Task CreateLocalizedBroadcast_FromXliff_ShouldCreateGermanDraft()
    {
        // Arrange
        var actions = new BroadcastActions(
            InvocationContext,
            FileManager);

        const string originalBroadcastId = "26340985";
        const string fileName = "kit-broadcast-26340985.html.xlf";
        const string targetLanguage = "de";

        var translatedFile = new FileReference
        {
            Name = fileName,
            ContentType = "application/xliff+xml"
        };

        // Act: Import the translated XLIFF and create a new Kit draft.
        var result = await actions.CreateLocalizedBroadcast(
            new CreateLocalizedBroadcastRequest
            {
                Content = translatedFile,
                TargetLanguage = targetLanguage
            });

        // Assert: A new draft was created.
        Assert.IsNotNull(result);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.Broadcast.Id));

        Assert.AreNotEqual(
            originalBroadcastId,
            result.Broadcast.Id,
            "The original broadcast must not be overwritten.");

        Assert.AreEqual("draft", result.Broadcast.Status);
        Assert.AreEqual("[de]", result.Broadcast.Description);

        // Retrieve the new broadcast to verify what Kit stored.
        var created = await actions.GetBroadcast(
            new BroadcastRequest
            {
                BroadcastId = result.Broadcast.Id
            });

        Assert.IsNotNull(
            result.Content,
            "The localized action should return an interoperable file.");

        Assert.AreEqual("de", result.TargetLanguage);

        // Verify that the returned XLIFF references the newly created Kit draft.
        // The test file manager writes uploads to Output,
        // but reads downloaded files from Input.
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var projectDirectory = Directory
            .GetParent(baseDirectory)?
            .Parent?
            .Parent?
            .Parent?
            .FullName
            ?? throw new Exception("Could not locate test project directory.");

        var outputPath = Path.Combine(
            projectDirectory,
            "TestFiles",
            "Output",
            result.Content!.Name);

        var inputPath = Path.Combine(
            projectDirectory,
            "TestFiles",
            "Input",
            result.Content.Name);

        Assert.IsTrue(
            File.Exists(outputPath),
            $"Returned XLIFF was not found: {outputPath}");

        File.Copy(outputPath, inputPath, overwrite: true);

        var returnedFile = await FileManager.DownloadAsync(result.Content);

        var returnedTransformation = Transformation.Load(
            returnedFile,
            result.Content!.Name,
            result.Content.ContentType);

        Assert.IsTrue(
            returnedTransformation.Success,
            $"Returned XLIFF could not be loaded: {returnedTransformation.Error}");

        Assert.AreEqual(
            result.Broadcast.Id,
            returnedTransformation.Value.TargetSystemReference.ContentId);

        Assert.AreEqual(
            result.Broadcast.Subject,
            returnedTransformation.Value.TargetSystemReference.ContentName);

        Assert.AreEqual(
            "Kit",
            returnedTransformation.Value.TargetSystemReference.SystemName);

        Assert.AreEqual(
            "de",
            returnedTransformation.Value.TargetLanguage);

        Console.WriteLine(
            $"Blacklake target reference: {returnedTransformation.Value.TargetSystemReference.ContentId}");

        Assert.AreEqual(result.Broadcast.Id, created.Id);

        Assert.IsFalse(
            string.IsNullOrWhiteSpace(created.Subject),
            "The translated subject should not be empty.");

        StringAssert.Contains(
            created.Content ?? string.Empty,
            "Dies ist eine englische Version.");

        Console.WriteLine($"Original broadcast ID: {originalBroadcastId}");
        Console.WriteLine($"German broadcast ID: {created.Id}");
        Console.WriteLine($"German subject: {created.Subject}");
        Console.WriteLine($"German description: {created.Description}");
        Console.WriteLine($"German content: {created.Content}");
    }


    [TestMethod]
    [TestCategory("Manual")]
    public async Task UpdateLocalizedBroadcast_ShouldUpdateExistingGermanDraft()
    {
        var actions = new BroadcastActions(
            InvocationContext,
            FileManager);

        const string sourceBroadcastId = "26340985";
        const string localizedBroadcastId = "26343184";
        const string targetLanguage = "de";
        const string fileName = "broadcast-update-de.html";

        // Retrieve the existing localized draft.
        var existing = await actions.GetBroadcast(new BroadcastRequest
        {
            BroadcastId = localizedBroadcastId
        });

        Assert.AreEqual("draft", existing.Status);

        // Prepare an updated translation using the original source metadata.
        var source = await actions.GetBroadcast(new BroadcastRequest
        {
            BroadcastId = sourceBroadcastId
        });

        var html = BroadcastHtmlHelper.PrepareHtml(source, "en-US");

        html = html
            .Replace("Integration Test Broadcast", "Aktualisierter Integrationstest")
            .Replace(
                "This is an English version.",
                "Dies ist die aktualisierte deutsche Version.");

        // Save the test HTML where our fake file manager expects it.
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var projectDirectory = Directory
            .GetParent(baseDirectory)?
            .Parent?
            .Parent?
            .Parent?
            .FullName
            ?? throw new Exception("Could not locate test project directory.");

        var inputDirectory = Path.Combine(
            projectDirectory,
            "TestFiles",
            "Input");

        Directory.CreateDirectory(inputDirectory);

        await File.WriteAllTextAsync(
            Path.Combine(inputDirectory, fileName),
            html,
            new System.Text.UTF8Encoding(false));

        // Update the existing draft.
        var result = await actions.UpdateLocalizedBroadcast(
            new UpdateLocalizedBroadcastRequest
            {
                BroadcastId = localizedBroadcastId,
                TargetLanguage = targetLanguage,
                Content = new FileReference
                {
                    Name = fileName,
                    ContentType = "text/html"
                }
            });

        // Verify that Kit updated the same broadcast.
        Assert.AreEqual(localizedBroadcastId, result.Broadcast.Id);
        Assert.AreEqual("draft", result.Broadcast.Status);
        Assert.AreEqual(targetLanguage, result.TargetLanguage);

        var updated = await actions.GetBroadcast(new BroadcastRequest
        {
            BroadcastId = localizedBroadcastId
        });

        Assert.AreEqual(
            "Aktualisierter Integrationstest",
            updated.Subject);

        StringAssert.Contains(
            updated.Content ?? string.Empty,
            "Dies ist die aktualisierte deutsche Version.");

        Assert.AreEqual(
            existing.Description,
            updated.Description,
            "The existing description should be preserved.");

        Console.WriteLine($"Updated broadcast ID: {updated.Id}");
        Console.WriteLine($"Updated subject: {updated.Subject}");
        Console.WriteLine($"Updated status: {updated.Status}");
        Console.WriteLine($"Updated description: {updated.Description}");
    }


    [TestMethod]
    [TestCategory("Manual")]
    public async Task UpdateLocalizedBroadcast_FromXliff_ShouldPreserveTargetReference()
    {
        var actions = new BroadcastActions(
            InvocationContext,
            FileManager);

        const string sourceBroadcastId = "26340985";
        const string localizedBroadcastId = "26343184";
        const string fileName = "kit-broadcast-26340985.html.xlf";
        const string targetLanguage = "de";

        // Confirm the existing localized broadcast is a draft.
        var existing = await actions.GetBroadcast(new BroadcastRequest
        {
            BroadcastId = localizedBroadcastId
        });

        Assert.AreEqual("draft", existing.Status);
        Assert.AreEqual("[de]", existing.Description);

        // Update the same Kit draft using translated XLIFF.
        var result = await actions.UpdateLocalizedBroadcast(
            new UpdateLocalizedBroadcastRequest
            {
                BroadcastId = localizedBroadcastId,
                TargetLanguage = targetLanguage,
                Content = new FileReference
                {
                    Name = fileName,
                    ContentType = "application/xliff+xml"
                }
            });

        // Confirm no new broadcast was created.
        Assert.AreEqual(localizedBroadcastId, result.Broadcast.Id);
        Assert.AreEqual("draft", result.Broadcast.Status);
        Assert.AreEqual(targetLanguage, result.TargetLanguage);
        Assert.IsNotNull(result.Content);

        // Verify the actual content stored in Kit.
        var updated = await actions.GetBroadcast(new BroadcastRequest
        {
            BroadcastId = localizedBroadcastId
        });

        Assert.AreEqual(
            "Sendung zum Integrationstest",
            updated.Subject);

        StringAssert.Contains(
            updated.Content ?? string.Empty,
            "Dies ist eine englische Version.");

        Assert.AreEqual("[de]", updated.Description);

        // Locate the returned XLIFF written by our test file manager.
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        var projectDirectory = Directory
            .GetParent(baseDirectory)?
            .Parent?
            .Parent?
            .Parent?
            .FullName
            ?? throw new Exception("Could not locate test project directory.");

        var outputPath = Path.Combine(
            projectDirectory,
            "TestFiles",
            "Output",
            result.Content!.Name);

        Assert.IsTrue(
            File.Exists(outputPath),
            $"Returned XLIFF not found: {outputPath}");

        // Load the returned XLIFF and inspect Blacklake metadata.
        using var returnedFile = File.OpenRead(outputPath);

        var transformationResult = Transformation.Load(
            returnedFile,
            result.Content.Name,
            result.Content.ContentType);

        Assert.IsTrue(
            transformationResult.Success,
            $"Returned XLIFF could not be loaded: {transformationResult.Error}");

        var transformation = transformationResult.Value;

        Assert.AreEqual(
            localizedBroadcastId,
            transformation.TargetSystemReference.ContentId);

        Assert.AreEqual(
            updated.Subject,
            transformation.TargetSystemReference.ContentName);

        Assert.AreEqual(
            "Kit",
            transformation.TargetSystemReference.SystemName);

        Assert.AreEqual(
            targetLanguage,
            transformation.TargetLanguage);

        // Verify the original English broadcast is still intact.
        var original = await actions.GetBroadcast(new BroadcastRequest
        {
            BroadcastId = sourceBroadcastId
        });

        Assert.AreEqual(
            "Integration Test Broadcast",
            original.Subject);

        StringAssert.Contains(
            original.Content ?? string.Empty,
            "This is an English version.");

        Console.WriteLine($"Source broadcast: {original.Id}");
        Console.WriteLine($"Updated broadcast: {updated.Id}");
        Console.WriteLine($"Updated subject: {updated.Subject}");
        Console.WriteLine($"Updated description: {updated.Description}");
        Console.WriteLine(
            $"Blacklake target reference: {transformation.TargetSystemReference.ContentId}");
        Console.WriteLine($"Target language: {transformation.TargetLanguage}");
    }
}
