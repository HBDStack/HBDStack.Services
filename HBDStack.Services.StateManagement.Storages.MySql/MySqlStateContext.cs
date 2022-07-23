using Microsoft.EntityFrameworkCore;

namespace HBDStack.Services.StateManagement.Storages.MySql;

internal class MySqlStateContext : DbContext
{
    public virtual DbSet<StateEntity> StateEntities { get; set; } = default!;
    
    public MySqlStateContext(DbContextOptions<MySqlStateContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<StateEntity>();
    }
}