namespace HBDStack.Services.StateManagement.Storages.PostgreSql.Tests.Data;

public class MyState
{
    public string? Name { get; set; }
    
    public DateTime Date { get; set; } = DateTime.Now;
}