namespace HBDStack.Services.FileStorage;

public enum ObjectTypes
{
    File,
    Directory
}
public record ObjectInfo(string Location,string FileName,long FileSize, DateTime CreatedDate, DateTime LastModifiedDate, ObjectTypes Type = ObjectTypes.File );