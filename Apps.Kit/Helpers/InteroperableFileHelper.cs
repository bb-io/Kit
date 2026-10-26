using System.Text;
using Apps.Kit.Constants;
using Apps.Kit.Extensions;
using Apps.Kit.Models.Html;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Constants;
using Blackbird.Filters.Shared;
using Blackbird.Filters.Transformations;

namespace Apps.Kit.Helpers;

public static class InteroperableFileHelper
{
    public static async Task<InteroperableHtmlContent> ReadAsync(IFileManagementClient fileManagementClient, FileReference file)
    {
        var bytes = await fileManagementClient.DownloadBytesAsync(file);
        using var stream = new MemoryStream(bytes);

        var loadResult = Transformation.Load(stream, file.Name, file.ContentType);
        if (!loadResult.Success)
            return new() { Html = Encoding.UTF8.GetString(bytes) };

        var targetResult = loadResult.Target();
        if (!targetResult.Success)
            throw new PluginMisconfigurationException($"Could not extract translated content: {targetResult.Error}");

        using var targetStream = targetResult.Value.ToStream();
        using var reader = new StreamReader(targetStream);

        return new()
        {
            Html = await reader.ReadToEndAsync(),
            Transformation = loadResult.Value,
            WasBilingual = loadResult.WasBilingual
        };
    }

    public static async Task<FileReference> UploadWithTargetReferenceAsync(
        IFileManagementClient fileManagementClient,
        InteroperableHtmlContent content,
        string contentId,
        string contentName,
        string targetLanguage,
        FileReference originalFile)
    {
        var transformation = content.Transformation;
        if (transformation is null)
            return originalFile;

        transformation.TargetSystemReference ??= new SystemReference();
        transformation.TargetSystemReference.ContentId = contentId;
        transformation.TargetSystemReference.ContentName = contentName;
        transformation.TargetSystemReference.SystemName = SystemReferences.SystemName;
        transformation.TargetSystemReference.SystemRef = SystemReferences.SystemRef;
        transformation.TargetLanguage = targetLanguage;

        return content.WasBilingual
            ? await UploadBilingualAsync(fileManagementClient, transformation)
            : await UploadTargetAsync(fileManagementClient, transformation);
    }

    private static async Task<FileReference> UploadBilingualAsync(
        IFileManagementClient fileManagementClient,
        Transformation transformation)
    {
        using var stream = transformation.ToStream();
        return await fileManagementClient.UploadAsync(stream, MediaTypes.Xliff2, transformation.BilingualFileName);
    }

    private static async Task<FileReference> UploadTargetAsync(
        IFileManagementClient fileManagementClient,
        Transformation transformation)
    {
        var targetResult = transformation.Target();
        if (!targetResult.Success)
            throw new PluginApplicationException($"Could not reconstruct localized content: {targetResult.Error}");

        var target = targetResult.Value;
        target.SystemReference = transformation.TargetSystemReference;

        using var stream = target.ToStream();
        return await fileManagementClient.UploadAsync(stream, target.OriginalMediaType, target.OriginalName);
    }
}
