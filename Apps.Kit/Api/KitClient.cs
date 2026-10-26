using Apps.Kit.Constants;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Kit.Api;

public class KitClient() : BlackBirdRestClient(new()
{
    BaseUrl = Urls.Api.ToUri()
})
{
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        return new PluginApplicationException(
            response.Content ?? response.ErrorMessage ?? "Kit API request failed.");
    }
}
