using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Storages.MySql.Tests.Fixtures;

public class StateManagementFixture : IAsyncDisposable
{
    public static string ConnectionString => "Server=dev-mysql-drunk.mysql.database.azure.com;Database=StateDb;Uid=dev4drunk;Pwd={Pwd};Allow User Variables=true;";
    
    public ServiceProvider Provider { get; }

    public StateManagementFixture()
    {
        Provider = new ServiceCollection()
            .AddMySqlStateStorage(ConnectionString)
            .BuildServiceProvider();
    }


    public async ValueTask DisposeAsync()
    {
        await Provider.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}