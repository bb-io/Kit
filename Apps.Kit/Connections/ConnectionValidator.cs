using System.Net;
using Apps.Kit.Api;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;
using RestSharp;

namespace Apps.Kit.Connections;

public class ConnectionValidator : IConnectionValidator
{
    private static readonly HttpStatusCode[] InvalidConnectionStatusCodes =
    [
        HttpStatusCode.Unauthorized,
        HttpStatusCode.NotFound
    ];

    public async ValueTask<ConnectionValidationResponse> ValidateConnection(
        IEnumerable<AuthenticationCredentialsProvider> authenticationCredentialsProviders,
        CancellationToken cancellationToken)
    {
        var request = new KitRequest("account", Method.Get, authenticationCredentialsProviders);
        var response = await new KitClient().ExecuteAsync(request, cancellationToken);

        if (!InvalidConnectionStatusCodes.Contains(response.StatusCode))
            return new() { IsValid = true };

        return new()
        {
            IsValid = false,
            Message = KitClient.ExtractErrorMessage(response)
        };
    }
}
