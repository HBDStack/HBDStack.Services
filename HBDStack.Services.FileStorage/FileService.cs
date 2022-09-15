using System.Text;
using HBDStack.Services.FileStorage.Abstracts;

namespace HBDStack.Services.FileStorage;

public class FileService : IFileService
{
    private readonly FileServiceOptions _options;
    private readonly IFileAdapter[] _adapters;

    public FileService(FileServiceOptions options, IEnumerable<IFileAdapter> adapters)
    {
        _options = options;
        _adapters = adapters.ToArray();
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

    protected virtual void ValidateFile(FileData file)
    {
        if (_options.MaxFileNameLength > 0 && file.Name.Length > _options.MaxFileNameLength)
            throw new FileLoadException("File name is invalid.");

        if (_options.IncludedExtensions != null)
        {
            var ext = Path.GetExtension(file.Name);
            if(string.IsNullOrEmpty(ext))
                throw new FileLoadException("File extension is invalid.");
            
            if(!_options.IncludedExtensions.Any(e=>string.Equals(e,ext,StringComparison.CurrentCultureIgnoreCase)))
                throw new FileLoadException("File extension is invalid.");
        }

        if (_options.MaxFileSizeInMb > 0)
        {
            var fileLength = file.Data.ToArray().LongLength;
            var limitLength = _options.MaxFileSizeInMb * 1000000; //Convert Mb to Byte
            if (fileLength > limitLength)
                throw new FileLoadException("File size is invalid.");
        }
    }

    public async Task<string> SaveFileAsync(FileData file, CancellationToken cancellationToken = default)
    {
        ValidateFile(file);
        //Save file to all adapters
        var fileLocation = GetFileLocation(file);
        var tasks = _adapters.Select(a =>
            a.SaveFileAsync(fileLocation, file.Data, file.OverwriteIfExisted, cancellationToken));
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