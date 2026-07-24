using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PAS.Infrastructure.Persistence;

/// <summary>
/// Design-time factory used by the EF Core tools (dotnet ef) to build an
/// <see cref="AssetDbContext"/> when scaffolding migrations, without having to
/// boot the whole application/host. The connection string here is only used for
/// provider metadata — generating a migration does not open a real connection.
/// </summary>
public sealed class AssetDbContextFactory : IDesignTimeDbContextFactory<AssetDbContext> {
    public AssetDbContext CreateDbContext(string[] args) {
        var options = new DbContextOptionsBuilder<AssetDbContext>()
            .UseSqlServer("Server=localhost;Database=PasAsset;Trusted_Connection=True;TrustServerCertificate=True")
            .Options;

        return new AssetDbContext(options);
    }
}
