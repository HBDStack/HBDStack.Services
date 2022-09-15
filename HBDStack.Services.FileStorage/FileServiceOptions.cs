namespace HBDStack.Services.FileStorage;

public class FileServiceOptions
{
    /// <summary>
    /// Whitelist file extensions
    /// </summary>
    public string[]? IncludedExtensions { get; set; }

    /// <summary>
    /// Limit the file name lenght. Any value <=0 will be unlimited
    /// </summary>
    public int MaxFileNameLength { get; set; } = 0;
    
    /// <summary>
    /// Limit the file Size. Any value <=0 will be unlimited
    /// </summary>
    public int MaxFileSizeInMb { get; set; } = 0;
}