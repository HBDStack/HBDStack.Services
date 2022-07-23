using HBDStack.Services.StateManagement.Storages.Sql.Tests.Fixtures;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Storages.Sql.Tests;

internal class SqlStateContextFactory : IDesignTimeDbContextFactory<SqlStateContext>
{
    #region Methods

    public SqlStateContext CreateDbContext(string[] args)
    {
        var service = new ServiceCollection()
            .AddSqlStateStorage(StateManagementFixture.ConnectionString)
            .BuildServiceProvider();

        return service.GetRequiredService<SqlStateContext>();
    }

    #endregion Methods
}