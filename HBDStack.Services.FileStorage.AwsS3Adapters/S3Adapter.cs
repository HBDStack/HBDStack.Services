using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using HBDStack.Services.FileStorage.Abstracts;
using Microsoft.Extensions.Options;

namespace HBDStack.Services.FileStorage.AwsS3Adapters;

public class S3Adapter : IFileAdapter
{
    private readonly S3Options _options;
    private IAmazonS3? _client;

    public S3Adapter(IOptions<S3Options> options) => _options = options.Value;

    private TransferUtility GetS3Client()
    {
        if (_client != null) return new TransferUtility(_client);
        
        if (!string.IsNullOrWhiteSpace(_options.AccessKey) && !string.IsNullOrWhiteSpace(_options.Secret))
            _client = new AmazonS3Client(new BasicAWSCredentials(_options.AccessKey, _options.Secret), RegionEndpoint.GetBySystemName(_options.RegionEndpointName));
        else _client = new AmazonS3Client(RegionEndpoint.GetBySystemName(_options.RegionEndpointName));

        return new TransferUtility(_client);
    }

    public Task SaveFileAsync(string fileLocation, BinaryData data, bool overwrite = false, CancellationToken cancellationToken = default)
    {
        using var transfer = GetS3Client();
        return transfer.UploadAsync(data.ToStream(),_options.BucketName,fileLocation, cancellationToken);
    }

    public Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        using var transfer = GetS3Client();
        return transfer.DownloadAsync(data.ToStream(),_options.BucketName,fileLocation, cancellationToken);
    }

    public Task<bool> DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> FileExistedAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}