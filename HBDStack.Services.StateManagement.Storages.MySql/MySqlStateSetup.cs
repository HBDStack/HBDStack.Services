using HBDStack.Services.StateManagement.Storages.MySql;
using Microsoft.EntityFrameworkCore;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class MySqlStateSetup
{
    public static IServiceCollection AddMySqlStateStorage(this IServiceCollection services, string connectionString)
    {
        return services
            .AddStateManagement()
            .AddDbContext<MySqlStateContext>(op => op.UseMySql(connectionString,ServerVersion.AutoDetect(connectionString),
                o => o.EnableRetryOnFailure()
                    .MigrationsAssembly(typeof(MySqlStateContext).Assembly.FullName)
                    .MigrationsHistoryTable($"migrate_{nameof(MySqlStateContext)}")
                    .UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)
            ), ServiceLifetime.Transient, ServiceLifetime.Singleton)
            .AddStateStorage<MySqlStateStorage>();
    }
}