using FluentAssertions;
using HBDStack.Services.StateManagement.Storages.PostgreSql.Tests.Data;
using HBDStack.Services.StateManagement.Storages.PostgreSql.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HBDStack.Services.StateManagement.Storages.PostgreSql.Tests;

public class SqlStateStorageTests : IClassFixture<StateManagementFixture>
{
    private readonly StateManagementFixture _fixture;

    public SqlStateStorageTests(StateManagementFixture fixture) => _fixture = fixture;

    private async Task<string?> GetValueFromCache(AsyncServiceScope service)
    {
        var key = $"STATEMANAGER/{typeof(SqlStateStorageTests).FullName}/{nameof(MyState)}".ToUpper();

        await using var context = service.ServiceProvider.GetRequiredService<PostgreSqlStateContext>();
        var item =await  context.StateEntities.FirstOrDefaultAsync(s => s.Id == key);

        return item?.Value;
    }


    [Fact]
    public async Task AddNewState()
    {
        await using var service = _fixture.Provider.CreateAsyncScope();
        var management =
            service.ServiceProvider.GetRequiredService<IStateManager<SqlStateStorageTests>>();

        //Modify the state
        var state = management.Get<MyState>();
        var value = await state.GetValueAsync();
        value.Name = "Hello";
        await state.CommitAsync();

        //Read State and Assert
        var savedState = await GetValueFromCache(service);
        savedState.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task ModifyStateWithoutCommits()
    {
        await using var service = _fixture.Provider.CreateAsyncScope();
        var management =
            service.ServiceProvider.GetRequiredService<IStateManager<SqlStateStorageTests>>();

        //Modify the state
        var state = management.Get<MyState>();
        var value = await state.GetValueAsync();
        value.Name = "Hello 1";
        await state.CommitAsync();

        value.Name = "Steven 1";

        //Read State and Assert
        var savedState = await GetValueFromCache(service);
        savedState.Should().Contain("Hello 1");

        //Commit and check again
        await state.CommitAsync();
        savedState = await GetValueFromCache(service);
        savedState.Should().Contain("Steven 1");
    }

    [Fact]
    public async Task RemoveState()
    {
        await using var service = _fixture.Provider.CreateAsyncScope();
        var management =
            service.ServiceProvider.GetRequiredService<IStateManager<SqlStateStorageTests>>();

        //Modify the state
        var state = management.Get<MyState>();
        var value = await state.GetValueAsync();
        value.Name = "Hello 1";

        await state.CommitAsync();

        //Read State and Assert
        var savedState = await GetValueFromCache(service);
        savedState.Should().NotBeNullOrEmpty();

        await state.RemoveAsync();

        //check again
        savedState = await GetValueFromCache(service);
        savedState.Should().BeNullOrWhiteSpace();
    }
}