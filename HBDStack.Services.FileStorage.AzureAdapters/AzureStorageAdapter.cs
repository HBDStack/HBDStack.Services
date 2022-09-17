using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HBDStack.Services.FileStorage.Abstracts;
using HBDStack.Services.FileStorage.Adapters;
using Microsoft.Extensions.Options;

namespace HBDStack.Services.FileStorage.AzureAdapters;

public class AzureStorageAdapter : IFileAdapter
{
    private readonly AzureStorageOptions _options;
    private BlobContainerClient? _containerClient;

    public AzureStorageAdapter(IOptions<AzureStorageOptions> options) =>
        _options = options.Value ?? throw new ArgumentException(nameof(AzureStorageOptions));

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

    public async Task<bool> DeleteFolderAsync(string folderLocation, CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        var resultSegment = client.GetBlobsAsync(BlobTraits.All, BlobStates.All, folderLocation).AsPages(default, 1000);

        await foreach (Azure.Page<BlobItem> blobPage in resultSegment.WithCancellation(cancellationToken))
        {
            var subFolders = blobPage.Values.Where(blobItem => blobItem.Metadata.ContainsKey("hdi_isfolder"));
            var files = blobPage.Values.Where(blobItem => subFolders.All(i => i.Name != blobItem.Name));

            foreach (var blobItem in files)
            {
                await DeleteFileAsync(blobItem.Name, cancellationToken);
            }

            foreach (var blobItem in subFolders)
            {
                await DeleteFileAsync(blobItem.Name, cancellationToken);
            }
        }

        return await DeleteFileAsync(folderLocation, cancellationToken);
    }

    public async Task<bool> FileExistedAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        var rs = await client.GetBlobClient(fileLocation).ExistsAsync(cancellationToken: cancellationToken);
        return rs.Value;
    }
}