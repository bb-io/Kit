using Apps.Kit.Constants;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Connections;

namespace Apps.Kit.Connections;

public class ConnectionDefinition : IConnectionDefinition
{
    public IEnumerable<ConnectionPropertyGroup> ConnectionPropertyGroups => new List<ConnectionPropertyGroup>
    {
        new()
        {
            Name = "API Key",
            AuthenticationType = ConnectionAuthenticationType.Undefined,
            ConnectionProperties = new List<ConnectionProperty>
            {
                new(CredNames.ApiKey)
                {
                    DisplayName = "API Key",
                    Sensitive = true,
                    Description = "The Kit V4 API key for authenticating requests to Kit."
                }
            }
        }
    };
    public IEnumerable<AuthenticationCredentialsProvider> CreateAuthorizationCredentialsProviders(
         Dictionary<string, string> values)
    {
        var apiKey = values.First(v => v.Key == CredNames.ApiKey);
        yield return new AuthenticationCredentialsProvider(apiKey.Key, apiKey.Value);
    }
}
