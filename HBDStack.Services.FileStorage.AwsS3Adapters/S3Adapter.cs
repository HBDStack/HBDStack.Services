using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using HBDStack.Services.FileStorage.Abstracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HBDStack.Services.FileStorage.AwsS3Adapters;

public class S3Adapter : IFileAdapter
{
    private readonly ILogger<S3Adapter> _logger;
    private readonly S3Options _options;
    private static IAmazonS3? _client;

    public S3Adapter(IOptions<S3Options> options, ILogger<S3Adapter> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    private TransferUtility GetS3Client()
    {
        if (_client != null) return new TransferUtility(_client);

        if (!string.IsNullOrWhiteSpace(_options.AccessKey) && !string.IsNullOrWhiteSpace(_options.Secret))
        {
            _client = new AmazonS3Client(new BasicAWSCredentials(_options.AccessKey, _options.Secret),
                RegionEndpoint.GetBySystemName(_options.RegionEndpointName));
            _logger.LogInformation("Loaded AmazonS3Client with BasicAWSCredentials");
        }
        else
        {
            _client = new AmazonS3Client(RegionEndpoint.GetBySystemName(_options.RegionEndpointName));
            _logger.LogInformation("Loaded AmazonS3Client without Credentials");
        }

        return new TransferUtility(_client);
    }

    public async Task SaveFileAsync(string fileLocation, BinaryData data, bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        var transfer = GetS3Client();
        var existed = await FileExistedAsync(fileLocation, cancellationToken);
        if (existed && !overwrite)
            throw new Exception($"File {fileLocation} is not allowed to override");

        await transfer.UploadAsync(data.ToStream(), _options.BucketName, fileLocation, cancellationToken);
    }

    public async Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var transfer = GetS3Client();
        try
        {
            var stream = await transfer.OpenStreamAsync(_options.BucketName, fileLocation, cancellationToken);
            return await BinaryData.FromStreamAsync(stream, cancellationToken);
        }
        catch (AmazonS3Exception e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
                return null;
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var transfer = GetS3Client();
        var existed = await FileExistedAsync(fileLocation, cancellationToken);
        if (!existed)
            throw new ArgumentNullException(nameof(fileLocation), $"File {fileLocation} was not found");

        var response =
            await transfer.S3Client.DeleteObjectAsync(_options.BucketName, fileLocation, cancellationToken);
        return response.HttpStatusCode == HttpStatusCode.NoContent;
    }

    public async Task<bool> DeleteFolderAsync(string folderLocation, CancellationToken cancellationToken = default)
    {
        var transfer = GetS3Client();
        var listObjectsResponse =
            await transfer.S3Client.ListObjectsAsync(_options.BucketName, folderLocation, cancellationToken);
        if (listObjectsResponse.HttpStatusCode != HttpStatusCode.OK) return false;

        foreach (var s3Object in listObjectsResponse.S3Objects)
        {
            await transfer.S3Client.DeleteObjectAsync(_options.BucketName, s3Object.Key, cancellationToken);
        }

        return true;
    }

    public async Task<bool> FileExistedAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var transfer = GetS3Client();
        try
        {
            var response =
                await transfer.S3Client.GetObjectMetadataAsync(_options.BucketName, fileLocation, cancellationToken);
            return response is { ContentLength: > 0 };
        }
        catch (AmazonS3Exception e)
        {
            if (e.StatusCode == HttpStatusCode.NotFound)
                return false;
            throw;
        }
    }
}