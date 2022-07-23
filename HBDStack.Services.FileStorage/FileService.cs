using System.Text;
using HBDStack.Services.FileStorage.Abstracts;
using Microsoft.Extensions.Logging;

namespace HBDStack.Services.FileStorage;

public class FileService : IFileService
{
    private readonly IFileAdapter[] _adapters;
    private readonly ILogger<FileService> _logger;

    public FileService(IEnumerable<IFileAdapter> adapters, ILogger<FileService> logger)
    {
        _adapters = adapters.ToArray();
        _logger = logger;

        if (!_adapters.Any()) throw new ArgumentException("No adapter found.");
    }

    private static string GetFileLocation(FileArgs file)
    {
        var builder = new StringBuilder();
        builder.Append(file.OwnerId);
        if (!string.IsNullOrWhiteSpace(file.Group))
        {
            if (!file.Group.StartsWith("/"))
                builder.Append('/');
            builder.Append(file.Group);
        }

        builder.Append('/').Append(file.Name);
        return builder.ToString();
    }

    public async Task<string> SaveFileAsync(FileData file, CancellationToken cancellationToken = default)
    {
        //Save file to all adapters
        var fileLocation = GetFileLocation(file);
        var tasks = _adapters.Select(a =>
            a.SaveFileAsync(fileLocation,file.Data, file.OverwriteIfExisted, cancellationToken));
        await Task.WhenAll(tasks);

        return fileLocation;
    }

    public Task<BinaryData?> GetFileAsync(FileArgs file, CancellationToken cancellationToken = default)
    {
        var fileLocation = GetFileLocation(file);
        return GetFileAsync(fileLocation, cancellationToken);
    }

    public async Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        //Get File from any adapter
        foreach (var adapter in _adapters)
        {
            var rs = await adapter.GetFileAsync(fileLocation, cancellationToken);
            if (rs != null)
                return rs;
        }

        return null;
    }

    public Task<bool> DeleteFileAsync(FileArgs file, CancellationToken cancellationToken = default)
    {
        //Delete file from all adapters
        var fileLocation = GetFileLocation(file);
        return DeleteFileAsync(fileLocation, cancellationToken);
    }

    public async Task<bool> DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var tasks = _adapters.Select(a =>
            a.DeleteFileAsync(fileLocation, cancellationToken));

        var rs = await Task.WhenAll(tasks);
        return rs.Any(r => r);
    }
}