using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace HBDStack.Services.StateManagement.Storages;

[SuppressMessage("Usage", "CA1816:Dispose methods should call SuppressFinalize")]
public class DistributedStateStorage : IStateStorage
{
    private readonly IDistributedCache _cache;

    public DistributedStateStorage(IDistributedCache cache) =>
        _cache = cache ?? throw new ArgumentNullException(nameof(cache));

    public ValueTask DisposeAsync()
    {
        Dispose();
        return ValueTask.CompletedTask;
    }

    public void Dispose()
    {
        //Ignored
    }


    public Task SetAsync<TState>(string key, TState state, CancellationToken cancellationToken = default)
        where TState : class
    {
        var value = JsonSerializer.SerializeToUtf8Bytes(state, new JsonSerializerOptions { WriteIndented = false });
        return _cache.SetAsync(key, value,
            new DistributedCacheEntryOptions { AbsoluteExpiration = DateTimeOffset.MaxValue }, cancellationToken);
    }

    public async Task<TState?> GetAsync<TState>(string key, CancellationToken cancellationToken = default)
        where TState : class
    {
        var bytes = await _cache.GetAsync(key, cancellationToken);
        if (bytes == null) return null;
        return JsonSerializer.Deserialize<TState>(bytes);
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default) => _cache.RemoveAsync(key, cancellationToken);

   
}