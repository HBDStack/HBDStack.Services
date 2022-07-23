using HBDStack.Services.StateManagement.Storages.MySql.Tests.Fixtures;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Storages.MySql.Tests;

internal class SqlStateContextFactory : IDesignTimeDbContextFactory<MySqlStateContext>
{
    #region Methods

    public MySqlStateContext CreateDbContext(string[] args)
    {
        var service = new ServiceCollection()
            .AddMySqlStateStorage(StateManagementFixture.ConnectionString)
            .BuildServiceProvider();

        return service.GetRequiredService<MySqlStateContext>();
    }

    #endregion Methods
}