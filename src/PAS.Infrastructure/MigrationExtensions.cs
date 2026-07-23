using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PAS.Infrastructure.Persistence;

namespace PAS.Infrastructure;

public static class MigrationExtensions {
    /// <summary>Applies any pending EF Core migrations for the <see cref="AssetDbContext"/>.</summary>
    public static void ApplyMigrations(this IServiceProvider serviceProvider) {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AssetDbContext>();
        dbContext.Database.Migrate();
    }
}
