using HBDStack.Services.FileStorage.AzureAdapters;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace  Microsoft.Extensions.DependencyInjection;

public static class AzureStorageSetup
{
    public static IServiceCollection AddAzureStorageAdapter(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<AzureStorageOptions>(o => configuration.GetSection(AzureStorageOptions.Name).Bind(o))
            .AddFileAdapter<AzureStorageAdapter>();
    
    public static IServiceCollection AddAzureStorageAdapter(this IServiceCollection services, AzureStorageOptions options)
        => services
            .AddSingleton(Options.Options.Create(options))
            .AddFileAdapter<AzureStorageAdapter>();
}