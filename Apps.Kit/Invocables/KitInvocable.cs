using Apps.Kit.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Kit.Invocables;

public class KitInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();

    protected KitClient Client { get; }

    protected KitInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();
    }
}
