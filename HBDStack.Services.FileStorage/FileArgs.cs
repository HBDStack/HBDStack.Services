namespace HBDStack.Services.FileStorage;

public record FileArgs
{
    public FileArgs(string ownerId, string name)
    {
        OwnerId = ownerId ?? throw new ArgumentNullException(nameof(ownerId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    /// <summary>
    /// The name of id of the owner of the file.
    /// </summary>
    public string OwnerId { get; init; }
    
    /// <summary>
    /// Custom group or path of the file if not provided it will store in the root of owner.
    /// </summary>
    public string? Group { get; set; }
    
    /// <summary>
    /// The name of the file
    /// </summary>
    public string Name { get; init; }
}