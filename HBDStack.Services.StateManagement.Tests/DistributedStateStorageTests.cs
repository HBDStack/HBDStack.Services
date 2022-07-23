using FluentAssertions;
using HBDStack.Services.StateManagement.Tests.Data;
using HBDStack.Services.StateManagement.Tests.Fixtures;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace HBDStack.Services.StateManagement.Tests;

public class DistributedStateStorageTests : IClassFixture<StateManagementFixture>
{
    private readonly StateManagementFixture _fixture;

    public DistributedStateStorageTests(StateManagementFixture fixture) => _fixture = fixture;

    private Task<string?> GetValueFromCache(AsyncServiceScope service)
    {
        var key = $"STATEMANAGER/{typeof(DistributedStateStorageTests).FullName}/{nameof(MyState)}".ToUpper();

        var cache = service.ServiceProvider.GetRequiredService<IDistributedCache>();
        return cache.GetStringAsync(key);
    }


    [Fact]
    public async Task AddNewState()
    {
        await using var service = _fixture.Provider.CreateAsyncScope();
        var management =
            service.ServiceProvider.GetRequiredService<IStateManager<DistributedStateStorageTests>>();

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
            service.ServiceProvider.GetRequiredService<IStateManager<DistributedStateStorageTests>>();

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
            service.ServiceProvider.GetRequiredService<IStateManager<DistributedStateStorageTests>>();

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