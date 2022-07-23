using HBDStack.Services.StateManagement.Storages.PostgreSql.Tests.Fixtures;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Storages.PostgreSql.Tests;

internal class SqlStateContextFactory : IDesignTimeDbContextFactory<PostgreSqlStateContext>
{
    #region Methods

    public PostgreSqlStateContext CreateDbContext(string[] args)
    {
        var service = new ServiceCollection()
            .AddPostgreSqlStateStorage(StateManagementFixture.ConnectionString)
            .BuildServiceProvider();

        return service.GetRequiredService<PostgreSqlStateContext>();
    }

    #endregion Methods
}