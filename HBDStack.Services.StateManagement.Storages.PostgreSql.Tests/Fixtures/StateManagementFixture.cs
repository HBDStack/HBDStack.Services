using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Storages.PostgreSql.Tests.Fixtures;

public class StateManagementFixture : IAsyncDisposable
{
    public static string ConnectionString =>
        "Server=dev-postgres-drunk.postgres.database.azure.com;Database=postgres;Port=5432;User Id=dev_postgres_drunk;Password={Password};Trust Server Certificate=true;";
    
    public ServiceProvider Provider { get; }

    public StateManagementFixture()
    {
        Provider = new ServiceCollection()
            .AddPostgreSqlStateStorage(ConnectionString)
            .BuildServiceProvider();
    }


    public async ValueTask DisposeAsync()
    {
        await Provider.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}