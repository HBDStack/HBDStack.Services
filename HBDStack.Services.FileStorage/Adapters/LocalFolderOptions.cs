namespace HBDStack.Services.FileStorage.Adapters;

public class LocalFolderOptions
{
    public static string Name => "FileService:LocalFolder";
    public string? RootFolder { get; set; }
}