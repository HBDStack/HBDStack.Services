using Microsoft.EntityFrameworkCore;

namespace HBDStack.Services.StateManagement.Storages.Sql;

internal class SqlStateContext : DbContext
{
    public virtual DbSet<StateEntity> StateEntities { get; set; } = default!;
    
    public SqlStateContext(DbContextOptions<SqlStateContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<StateEntity>();
    }
}