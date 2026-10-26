using System.Text;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Kit.Extensions;

public static class FileManagementClientExtensions
{
    public static async Task<byte[]> DownloadBytesAsync(this IFileManagementClient client, FileReference file)
    {
        var stream = await client.DownloadAsync(file);
        return await stream.GetByteData();
    }

    public static async Task<string> DownloadStringAsync(this IFileManagementClient client, FileReference file)
    {
        var bytes = await client.DownloadBytesAsync(file);
        return Encoding.UTF8.GetString(bytes);
    }

    public static async Task<FileReference> UploadStringAsync(
        this IFileManagementClient client,
        string content,
        string contentType,
        string fileName)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        return await client.UploadAsync(stream, contentType, fileName);
    }
}
