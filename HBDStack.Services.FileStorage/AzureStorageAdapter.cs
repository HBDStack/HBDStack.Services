using Azure.Storage.Blobs;
using HBDStack.Services.FileStorage.Abstracts;
using HBDStack.Services.FileStorage.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HBDStack.Services.FileStorage;

public class AzureStorageAdapter : IFileAdapter
{
    private readonly ILogger<AzureStorageAdapter> _logger;
    private readonly AzureStorageOptions _options;
    private BlobContainerClient? _containerClient;

    public AzureStorageAdapter(IOptions<AzureStorageOptions> options, ILogger<AzureStorageAdapter> logger)
    {
        _logger = logger;
        _options = options.Value ?? throw new ArgumentException(nameof(AzureStorageOptions));
    }

    private async Task<BlobContainerClient> GetBlobClient()
    {
        if (_containerClient != null) return _containerClient;

        var blobClient = new BlobServiceClient(_options.ConnectionString);
        _containerClient = blobClient.GetBlobContainerClient(_options.ContainerName);
        await _containerClient.CreateIfNotExistsAsync();

        return _containerClient;
    }

    public async Task SaveFileAsync(string fileLocation, BinaryData data, bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        await client.GetBlobClient(fileLocation).UploadAsync(data, overwrite, cancellationToken);
    }

    public async Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        var blob = client.GetBlobClient(fileLocation);
        var es = await blob.ExistsAsync(cancellationToken);
        if (!es.Value) return null;

        var rs = await client.GetBlobClient(fileLocation).DownloadContentAsync(cancellationToken);
        return rs.Value!.Content;
    }

    public async Task<bool> DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        var rs = await client.GetBlobClient(fileLocation).DeleteIfExistsAsync(cancellationToken: cancellationToken);
        return rs.Value;
    }

    public async Task<bool> FileExistedAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        var rs = await client.GetBlobClient(fileLocation).ExistsAsync(cancellationToken: cancellationToken);
        return rs.Value;
    }
}