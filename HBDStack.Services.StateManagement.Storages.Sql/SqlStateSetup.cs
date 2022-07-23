using HBDStack.Services.StateManagement.Storages.Sql;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class SqlStateSetup
{
    public static IServiceCollection AddSqlStateStorage(this IServiceCollection services, string connectionString)
    {
        return services
            .AddStateManagement()
            .AddDbContext<SqlStateContext>(op => op.UseSqlServer(connectionString,
                o => o.EnableRetryOnFailure()
                    .MigrationsAssembly(typeof(SqlStateContext).Assembly.FullName)
                    .MigrationsHistoryTable(nameof(SqlStateContext), "migrate")
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            ), ServiceLifetime.Transient, ServiceLifetime.Singleton)
            .AddStateStorage<SqlStateStorage>();
    }
}