namespace HBDStack.Services.FileStorage.Abstracts;

public interface IFileService
{
    Task<string> SaveFileAsync(FileData file,CancellationToken cancellationToken = default);
    Task<BinaryData?> GetFileAsync(FileArgs file,CancellationToken cancellationToken = default);
    Task<BinaryData?> GetFileAsync(string fileLocation, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(FileArgs file,CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileLocation,CancellationToken cancellationToken = default);
}