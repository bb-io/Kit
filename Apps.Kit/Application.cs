using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Kit;

public class Application : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.Marketing];
        set { }
    }

    public string Name
    {
        get => "Kit";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}
