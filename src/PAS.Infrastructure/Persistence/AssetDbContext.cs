using Microsoft.EntityFrameworkCore;
using PAS.Domain.Funds;

namespace PAS.Asset.Infrastructure.Persistence;

public sealed class AssetDbContext : DbContext {
    public AssetDbContext(DbContextOptions<AssetDbContext> options) : base(options) { }

    public DbSet<Fund> Funds => Set<Fund>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssetDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}