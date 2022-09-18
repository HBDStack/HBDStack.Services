using System.Runtime.CompilerServices;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HBDStack.Services.FileStorage.Abstracts;
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

    public async IAsyncEnumerable<ObjectInfo> ListObjectInfoAsync(string location, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        location = location.RemoveHeadingSlash();
        var resultSegment = client.GetBlobsAsync(BlobTraits.None, BlobStates.All, location, cancellationToken);

        await foreach (var blob in resultSegment)
        {
            yield return new ObjectInfo(location, blob.Name,
                blob.Properties.ContentLength!.Value,
                blob.Properties.CreatedOn!.Value.LocalDateTime,
                blob.Properties.LastModified!.Value.LocalDateTime,
                blob.IsDirectory() ? ObjectTypes.Directory : ObjectTypes.File);
        }
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
        var queue = new Queue<string>();
        var subStack = new Stack<string>();
        queue.Enqueue(folderLocation.EnsureTrailingSlash());

        while (queue.Count > 0)
        {
            var tbDelete = queue.Dequeue();
            var resultSegment = client.GetBlobsAsync(BlobTraits.None, BlobStates.All, tbDelete, cancellationToken);

            //Delete Files
            await foreach (var blob in resultSegment.WithCancellation(cancellationToken))
            {
                if (blob.IsDirectory())
                    queue.Enqueue(blob.Name.EnsureTrailingSlash());
                else await DeleteFileAsync(blob.Name, cancellationToken);
            }

            //Add Empty folder to stack and delete later
            subStack.Push(tbDelete);
        }

        //Delete all empty Subfolders and folder
        while (subStack.Count > 0)
        {
            await DeleteFileAsync(subStack.Pop(), cancellationToken);
        }

        //Tobe True or Exception.
        return true;
    }

    public async Task<bool> FileExistedAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var client = await GetBlobClient();
        var rs = await client.GetBlobClient(fileLocation).ExistsAsync(cancellationToken: cancellationToken);
        return rs.Value;
    }
}