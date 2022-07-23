using HBDStack.Services.FileStorage;
using HBDStack.Services.FileStorage.Abstracts;
using HBDStack.Services.FileStorage.Options;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace  Microsoft.Extensions.DependencyInjection;

public static class FileServiceSetup
{
    public static IServiceCollection AddFileService(this IServiceCollection services)
    {
        if (services.Any(s => s.ServiceType == typeof(IFileService))) return services;
        return services.AddSingleton<IFileService, FileService>();
    }

    public static IServiceCollection AddFileAdapter<TAdapter>(this IServiceCollection services)
        where TAdapter : class, IFileAdapter
    {
        if (services.Any(s => s.ImplementationType == typeof(TAdapter))) return services;
        return services.AddSingleton<IFileAdapter, TAdapter>();
    }

    public static IServiceCollection AddLocalFolderAdapter(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<LocalFolderOptions>(o => configuration.GetSection(LocalFolderOptions.Name).Bind(o))
            .AddFileAdapter<LocalFolderFileAdapter>();

    public static IServiceCollection AddAzureStorageAdapter(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<AzureStorageOptions>(o => configuration.GetSection(AzureStorageOptions.Name).Bind(o))
            .AddFileAdapter<AzureStorageAdapter>();
}