using Microsoft.Extensions.DependencyInjection;

namespace PAS.Asset.Application;

public static class DependencyInjection {
    public static IServiceCollection AddApplication(this IServiceCollection services) {
        // Scans this assembly and registers every IRequestHandler automatically.
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        return services;
    }
}