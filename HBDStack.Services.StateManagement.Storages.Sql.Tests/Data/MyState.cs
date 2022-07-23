namespace HBDStack.Services.StateManagement.Storages.Sql.Tests.Data;

public class MyState
{
    public string? Name { get; set; }
    
    public DateTime Date { get; set; } = DateTime.Now;
}