namespace HBDStack.Services.FileStorage.AwsS3Adapters;

public class S3Options
{
    public static string Name => "FileService:S3";
    
    public string ConnectionString { get; set; }= default!;
    
    /// <summary>
    /// The Name of RegionEnd ex: ap-southeast-1, ap-east-1 or ca-central-1 ...
    /// </summary>
    public string RegionEndpointName { get; set; } = "ap-southeast-1";

    public string BucketName { get; set; } = default!;
    
    public string? AccessKey { get; set; }
    public string? Secret { get; set; }
}