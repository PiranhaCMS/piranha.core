

using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Cms.Services;

public static class AeroStartupExtensions
{
    public static IServiceCollection AddAero(this IServiceCollection services,
        Action<AeroServiceBuilder> options = null, ServiceLifetime scope = ServiceLifetime.Scoped)
    {
        var serviceBuilder = new AeroServiceBuilder(services);

        options?.Invoke(serviceBuilder);

        services.AddSingleton<IContentFactory, ContentFactory>();

        services.AddScoped<IApi, Api>();
        services.AddScoped<Config>();

        return services;
    }
}