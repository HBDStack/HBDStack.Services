using System.Collections.Concurrent;
using HBDStack.Services.StateManagement.Storages;

namespace HBDStack.Services.StateManagement;

public interface IStateManager<TClass> where TClass : class
{
    IStateService<TState> Get<TState>() where TState : class, new();

    public ValueTask<TState> GetValueAsync<TState>() where TState : class, new()
        => Get<TState>().GetValueAsync();

    Task RemoveAsync<TState>() where TState : class, new();
}

public class StateManager<TClass> : IStateManager<TClass> where TClass : class
{
    private readonly IStateStorage _storage;
    private readonly string _name;
    private readonly ConcurrentDictionary<string, dynamic> _cache = new();

    public StateManager(IStateStorage storage)
    {
        _name = $"StateManager/{typeof(TClass).FullName ?? typeof(TClass).Name}"; 
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    protected virtual string GetCacheKey<TState>() => typeof(TState).FullName ?? typeof(TState).Name;

    protected virtual IStateService<TState> CreateService<TState>()
        where TState : class, new() =>
        new StateService<TState>(_name, _storage);

    public IStateService<TState> Get<TState>() where TState : class, new()
    {
        var name = GetCacheKey<TState>();
        return _cache.GetOrAdd(name, n => CreateService<TState>());
    }

    public virtual async Task RemoveAsync<TState>() where TState : class, new()
    {
        var service = Get<TState>();
        await service.RemoveAsync();

        var name = GetCacheKey<TState>();
        _cache.TryRemove(new KeyValuePair<string, dynamic>(name, service));
    }
}