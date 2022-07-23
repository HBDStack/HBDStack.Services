using HBDStack.Services.StateManagement.Storages.PostgreSql;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class PostgreSqlStateSetup
{
    public static IServiceCollection AddPostgreSqlStateStorage(this IServiceCollection services, string connectionString)
    {
        return services
            .AddStateManagement()
            .AddDbContext<PostgreSqlStateContext>(op => op.UseNpgsql(connectionString,
                o => o.EnableRetryOnFailure()
                    .MigrationsAssembly(typeof(PostgreSqlStateContext).Assembly.FullName)
                    .MigrationsHistoryTable(nameof(PostgreSqlStateContext), "migrate")
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            ), ServiceLifetime.Transient, ServiceLifetime.Singleton)
            .AddStateStorage<PostgreSqlStateStorage>();
    }
}