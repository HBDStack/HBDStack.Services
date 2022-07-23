namespace HBDStack.Services.FileStorage;

public record FileData : FileArgs
{
    public FileData(string ownerId, string name, BinaryData data) : base(ownerId, name) 
        => Data = data ?? throw new ArgumentNullException(nameof(data));

    public BinaryData Data { get; init; }
    
    public bool OverwriteIfExisted { get; set; }
}