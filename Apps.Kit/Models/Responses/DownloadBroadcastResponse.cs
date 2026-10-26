using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Kit.Models.Responses;

public class DownloadBroadcastResponse
{
    public FileReference Content { get; set; } = new();
}