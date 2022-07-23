using HBDStack.Services.StateManagement;
using HBDStack.Services.StateManagement.Storages;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class StateSetup
{
    public static IServiceCollection AddStateManagement(this IServiceCollection services)
    {
        if (services.Any(s => s.ServiceType == typeof(IStateManager<>))) return services;
        return services.AddSingleton(typeof(IStateManager<>), typeof(StateManager<>));
    }
    
    public static IServiceCollection AddStateStorage<TImplementation>(this IServiceCollection services)where TImplementation:class,IStateStorage
    {
        if (services.Any(s => s.ServiceType == typeof(IStateStorage)))
            throw new InvalidOperationException($"The {nameof(IStateStorage)} is already existed.");
        
        return services.AddSingleton<IStateStorage, TImplementation>();
    }

    /// <summary>
    /// Distributed State Storage is using Distributed Cache Service. Ensure you added before use this.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddDistributedStateStorage(this IServiceCollection services)
    {
        return services
            .AddStateManagement()
            .AddStateStorage<DistributedStateStorage>();
    }
}