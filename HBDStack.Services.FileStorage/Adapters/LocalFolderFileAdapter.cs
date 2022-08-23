using HBDStack.Services.FileStorage.Abstracts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HBDStack.Services.FileStorage.Adapters;

public class LocalFolderFileAdapter : IFileAdapter
{
    private readonly ILogger<LocalFolderFileAdapter> _logger;
    private readonly string _rootFolder;

    public LocalFolderFileAdapter(IOptions<LocalFolderOptions> options, ILogger<LocalFolderFileAdapter> logger)
    {
        _logger = logger;
        _rootFolder = options.Value.RootFolder ?? Directory.GetCurrentDirectory();
    }

    private string GetFinalPath(string fileLocation)
    {
        if (fileLocation.StartsWith("/")) fileLocation = fileLocation[1..];
        return Path.GetFullPath(Path.Combine(_rootFolder, fileLocation));
    }

    public async Task SaveFileAsync(string fileLocation, BinaryData data, bool overwrite = false,
        CancellationToken cancellationToken = default)
    {
        var finalFile = GetFinalPath(fileLocation);
        var directory = Path.GetDirectoryName(finalFile);

        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory!);

        if (await FileExistedAsync(fileLocation, cancellationToken) && !overwrite)
            throw new InvalidOperationException("File already existed");

        await File.WriteAllBytesAsync(finalFile, data.ToArray(), cancellationToken);
    }

    public async Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var finalFile = GetFinalPath(fileLocation);
        if (!File.Exists(finalFile)) return null;

        var bytes = await File.ReadAllBytesAsync(finalFile, cancellationToken);
        return BinaryData.FromBytes(bytes);
    }

    public Task<bool> DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var finalFile = GetFinalPath(fileLocation);

        try
        {
            File.Delete(finalFile);
            return Task.FromResult(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistedAsync(string fileLocation, CancellationToken cancellationToken = default)
    {
        var finalFile = GetFinalPath(fileLocation);
        return Task.FromResult(File.Exists(finalFile));
    }
}