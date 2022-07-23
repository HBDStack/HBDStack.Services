using Microsoft.EntityFrameworkCore;

namespace HBDStack.Services.StateManagement.Storages.PostgreSql;

internal class PostgreSqlStateContext : DbContext
{
    public virtual DbSet<StateEntity> StateEntities { get; set; } = default!;
    
    public PostgreSqlStateContext(DbContextOptions<PostgreSqlStateContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<StateEntity>();
    }
}