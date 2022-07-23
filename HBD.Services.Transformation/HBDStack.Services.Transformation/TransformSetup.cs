using HBDStack.Services.Transformation;

// ReSharper disable CheckNamespace
// ReSharper disable UnusedMember.Global

namespace Microsoft.Extensions.DependencyInjection;

public static class TransformSetup
{
    public static IServiceCollection AddTransformerService(this IServiceCollection services, Action<TransformOptions> optionFactory)
        => services.AddTransient<ITransformerService>(p => new TransformerService(optionFactory));
}