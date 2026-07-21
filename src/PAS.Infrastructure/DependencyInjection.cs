using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Asset.Infrastructure.Persistence;
using PAS.Asset.Infrastructure.Persistence.Repositories;
using PAS.Domain.Repositories;

namespace PAS.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        var connectionString = configuration.GetConnectionString("PasAsset");

        services.AddDbContext<AssetDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IFundRepository, FundRepository>();

        return services;
    }
}