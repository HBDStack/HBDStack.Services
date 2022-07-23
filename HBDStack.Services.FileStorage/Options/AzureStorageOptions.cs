namespace HBDStack.Services.FileStorage.Options;

public class AzureStorageOptions
{
    public static string Name => "FileService:AzureStorage";

    public string ConnectionString { get; set; } = default!;
    public string ContainerName { get; set; } = default!;
}