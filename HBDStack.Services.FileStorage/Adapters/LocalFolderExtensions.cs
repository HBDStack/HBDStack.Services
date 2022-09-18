namespace HBDStack.Services.FileStorage.Adapters;

public static class LocalFolderExtensions
{
    public static bool IsDirectory(this string path)
    {
        try
        {
            var attr = File.GetAttributes(path);
            return attr.HasFlag(FileAttributes.Directory);
        }
        catch
        {
            return false;
        }
    }
}