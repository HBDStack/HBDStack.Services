namespace HBDStack.Services.FileStorage.Abstracts;

public interface IFileService
{
    Task<string> SaveFileAsync(FileData file, CancellationToken cancellationToken = default);
    Task<BinaryData?> GetFileAsync(FileArgs file, CancellationToken cancellationToken = default);
    Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default);
    Task<ObjectInfo?> GetObjectInfoAsync(FileArgs file, CancellationToken cancellationToken = default);
    Task<ObjectInfo?> GetObjectInfoAsync(string fileLocation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get File Headers the location shall be a file or directory
    /// </summary>
    /// <param name="location"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<ObjectInfo> ListObjectInfoAsync(string location, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteFileAsync(FileArgs file, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default);
}