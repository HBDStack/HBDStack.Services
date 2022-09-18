namespace HBDStack.Services.FileStorage.Abstracts;

public interface IFileAdapter
{
    Task SaveFileAsync(string fileLocation, BinaryData data, bool overwrite = false, CancellationToken cancellationToken = default);
    Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get File Headers the location shall be a file or directory
    /// </summary>
    /// <param name="location"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    IAsyncEnumerable<ObjectInfo> ListObjectInfoAsync(string location, CancellationToken cancellationToken = default);
    
    Task<bool> DeleteFileAsync(string fileLocation, CancellationToken cancellationToken = default);
    Task<bool> DeleteFolderAsync(string folderLocation, CancellationToken cancellationToken = default);
    Task<bool> FileExistedAsync(string fileLocation, CancellationToken cancellationToken = default);
}