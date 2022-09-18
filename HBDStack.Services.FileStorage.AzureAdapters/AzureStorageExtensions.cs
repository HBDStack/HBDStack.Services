using Azure.Storage.Blobs.Models;

namespace HBDStack.Services.FileStorage.AzureAdapters;

public static class AzureStorageExtensions
{
    public static string EnsureTrailingSlash(this string path) => path.EndsWith("/") ? path : $"{path}/";

    public static string RemoveHeadingSlash(this string path) => path.StartsWith("/") ? path[1..] : path;
    
    public static bool IsDirectory(this BlobItem blob) => blob.Properties.ContentLength <= 0 && string.IsNullOrEmpty(blob.Properties.ContentType);
}