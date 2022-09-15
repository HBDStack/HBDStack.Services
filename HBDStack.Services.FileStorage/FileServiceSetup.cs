using HBDStack.Services.FileStorage;
using HBDStack.Services.FileStorage.Abstracts;
using HBDStack.Services.FileStorage.Adapters;
using Microsoft.Extensions.Configuration;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection;

public static class FileServiceSetup
{
    public static IServiceCollection AddFileService(this IServiceCollection services, FileServiceOptions? options = null)
    {
        if (services.Any(s => s.ServiceType == typeof(IFileService))) return services;

        services
            .AddSingleton(options ?? new FileServiceOptions())
            .AddSingleton<IFileService, FileService>();

        return services;
    }

    public static IServiceCollection AddFileAdapter<TAdapter>(this IServiceCollection services)
        where TAdapter : class, IFileAdapter =>
        services.Any(s => s.ImplementationType == typeof(TAdapter)) ? services : services.AddSingleton<IFileAdapter, TAdapter>();

    public static IServiceCollection AddLocalFolderAdapter(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<LocalFolderOptions>(o => configuration.GetSection(LocalFolderOptions.Name).Bind(o))
            .AddFileAdapter<LocalFolderFileAdapter>();
}