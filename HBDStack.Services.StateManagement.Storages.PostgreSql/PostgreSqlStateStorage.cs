using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.StateManagement.Storages.PostgreSql;

internal class PostgreSqlStateStorage : IStateStorage
{
    private static bool _initialized = false;
    private readonly IServiceScope _provider;

    public PostgreSqlStateStorage(IServiceProvider provider) => _provider = provider.CreateScope();

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    public void Dispose() => _provider.Dispose();
    
    private async Task<PostgreSqlStateContext> GetSqlStateContext()
    {
        var context = _provider.ServiceProvider.GetRequiredService<PostgreSqlStateContext>();
        if(!_initialized)
        {
            _initialized = true;
            await context.Database.MigrateAsync();
        }
        return context;
    }

    public async Task SetAsync<TState>(string key, TState state, CancellationToken cancellationToken = default)
        where TState : class
    {
        await using var context = await GetSqlStateContext();

        var value = JsonSerializer.Serialize(state);
        var entity = await context.FindAsync<StateEntity>(key);

        if (entity == null)
        {
            entity = new StateEntity(key, value);
            await context.AddAsync(entity, cancellationToken);
        }
        else entity.UpdateValue(value);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TState?> GetAsync<TState>(string key, CancellationToken cancellationToken = default)
        where TState : class
    {
        await using var context = await GetSqlStateContext();

        var entity = await context.FindAsync<StateEntity>(key);
        return entity != null ? JsonSerializer.Deserialize<TState>(entity.Value) : null;
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        await using var context = await GetSqlStateContext();

        var entity = await context.FindAsync<StateEntity>(key);
        if (entity != null)
        {
            context.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }
}