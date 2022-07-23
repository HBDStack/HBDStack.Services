using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HBDStack.Services.StateManagement.Storages.PostgreSql;

[Table("Entities", Schema = "state")]
internal class StateEntity
{
    public StateEntity(string id, string value)
    {
        Id = id;
        Value = value;
        CreatedOn = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    }

    private StateEntity()
    {
    }

    [Required, Key, MaxLength(250)] public string Id { get; private set; } = default!;

    [Required] public string Value { get; private set; } = default!;

    public DateTimeOffset CreatedOn { get; private set; } = default!;
    public DateTimeOffset? UpdatedOn { get; private set; }

    public void UpdateValue(string value)
    {
        Value = value;
        UpdatedOn =  DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
    }
}