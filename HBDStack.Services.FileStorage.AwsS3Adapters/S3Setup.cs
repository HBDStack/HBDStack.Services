using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HBDStack.Services.FileStorage.AwsS3Adapters;

public static class S3Setup
{
    public static IServiceCollection AddS3Adapter(this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<S3Options>(o => configuration.GetSection(S3Options.Name).Bind(o))
            .AddFileAdapter<S3Adapter>();
}