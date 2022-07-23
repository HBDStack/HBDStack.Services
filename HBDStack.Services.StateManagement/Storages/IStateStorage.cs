namespace HBDStack.Services.StateManagement.Storages;

public interface IStateStorage : IAsyncDisposable, IDisposable
{
    Task SetAsync<TState>(string key, TState state, CancellationToken cancellationToken = default) where TState : class;
    Task<TState?> GetAsync<TState>(string key, CancellationToken cancellationToken = default) where TState : class;
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}