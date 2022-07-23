using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Storages.Sql.Tests.Fixtures;

public class StateManagementFixture : IAsyncDisposable
{
    public static string ConnectionString =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=StateDb;Integrated Security=True;Connect Timeout=30;"
            : "Data Source=localhost;Initial Catalog=StateDb;User Id=sa;Password=Pass@word1;";
    
    
    public ServiceProvider Provider { get; }

    public StateManagementFixture()
    {
        Provider = new ServiceCollection()
            .AddSqlStateStorage(ConnectionString)
            .BuildServiceProvider();
    }


    public async ValueTask DisposeAsync()
    {
        await Provider.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}