using Apps.Kit.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Utils.Extensions.Sdk;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using RestSharp;

namespace Apps.Kit.Api;

public class KitRequest(string resource, Method method, IEnumerable<AuthenticationCredentialsProvider> creds)
    : BlackBirdRestRequest(resource, method, creds)
{
    protected override void AddAuth(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var token = creds.Get(CredNames.ApiKey).Value;
        this.AddHeader("X-Kit-Api-Key", token);
    }
}
