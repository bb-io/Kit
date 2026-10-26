using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Microsoft.Extensions.Configuration;

namespace Tests.Kit.Base;

public class TestBase
{
    protected TestBase()
    {
        var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        Creds = config.GetSection("ConnectionDefinition").GetChildren()
            .Select(x => new AuthenticationCredentialsProvider(x.Key, x.Value!)).ToList();

        InvocationContext = new InvocationContext
        {
            AuthenticationCredentialsProviders = Creds,
        };

        FileManager = new FileManager();
    }

    protected IEnumerable<AuthenticationCredentialsProvider> Creds { get; set; }

    public InvocationContext InvocationContext { get; set; }

    protected FileManager FileManager { get; set; }
}
