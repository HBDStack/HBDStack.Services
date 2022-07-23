using HBDStack.Services.StateManagement.Storages;

namespace HBDStack.Services.StateManagement;

public interface IStateService<TState> where TState : class
{
    ValueTask<TState> GetValueAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// The commit the changes to the storage.
    /// </summary>
    /// <returns></returns>
    Task CommitAsync();

    /// <summary>
    /// Remove State from Storage.
    /// </summary>
    /// <returns></returns>
    Task RemoveAsync();
}

public class StateService<TState> : IStateService<TState> where TState : class, new()
{
    private readonly string _keyName;
    private readonly IStateStorage _storage;
    private TState? _current;

    public StateService(string managerName, IStateStorage storage, TState? defaultSate = null)
    {
        if (managerName == null) throw new ArgumentNullException(nameof(managerName));

        var stateName = typeof(TState).Name;
        _keyName = $"{managerName}/{stateName}".ToUpper();
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        _current = defaultSate;
    }

    public virtual async ValueTask<TState> GetValueAsync(CancellationToken cancellationToken = default)
    {
        if (_current != null) return _current;
        _current = await _storage.GetAsync<TState>(_keyName, cancellationToken) ?? new TState();
        return _current;
    }

    public async Task CommitAsync()
    {
        if (_current == null) return;
        await _storage.SetAsync(_keyName, _current);
    }

    public virtual async Task RemoveAsync()
    {
        _current = null;
        await _storage.RemoveAsync(_keyName);
    }
}