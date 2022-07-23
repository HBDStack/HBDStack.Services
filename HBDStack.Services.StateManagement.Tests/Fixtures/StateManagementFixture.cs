using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Tests.Fixtures;

public class StateManagementFixture : IAsyncDisposable
{
    public ServiceProvider Provider { get; }

    public StateManagementFixture()
    {
        Provider = new ServiceCollection()
            .AddDistributedMemoryCache()
            .AddDistributedStateStorage()
            .BuildServiceProvider();
    }


    public async ValueTask DisposeAsync()
    {
        await Provider.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}