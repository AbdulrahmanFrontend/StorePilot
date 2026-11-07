using Microsoft.EntityFrameworkCore;
using StorePilot.Domain.Entities;

namespace StorePilot.Infrastructure.Data;

public sealed class StorePilotDbContext : DbContext
{
    public StorePilotDbContext(
        DbContextOptions<StorePilotDbContext> options)
        : base(options)
    {
    }

    public DbSet<Unit> Units => Set<Unit>();
    public DbSet<ProductGroup> ProductGroups => Set<ProductGroup>();
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(StorePilotDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
