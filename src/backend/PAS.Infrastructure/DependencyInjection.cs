using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PAS.Infrastructure.Persistence;
using PAS.Infrastructure.Persistence.Interceptors;
using PAS.Infrastructure.Persistence.Repositories;
using PAS.Domain.Repositories;

namespace PAS.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration) {
        var connectionString = configuration.GetConnectionString("PasAsset");

        services.AddScoped<DomainEventsInterceptor>();

        services.AddDbContext<AssetDbContext>((serviceProvider, options) =>
            options
                .UseSqlServer(connectionString)
                .AddInterceptors(serviceProvider.GetRequiredService<DomainEventsInterceptor>()));

        services.AddScoped<IFundRepository, FundRepository>();

        return services;
    }
}