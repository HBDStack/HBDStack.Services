namespace HBDStack.Services.FileStorage.Options;

public class LocalFolderOptions
{
    public static string Name => "FileService:LocalFolder";
    public string? RootFolder { get; set; }
}