

using Microsoft.Extensions.DependencyInjection;
using Aero.Cms;
using Aero.Cms.ImageSharp;

public static class ImageSharpExtensions
{
    public static AeroServiceBuilder UseImageSharp(this AeroServiceBuilder serviceBuilder,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        serviceBuilder.Services.AddAeroImageSharp(scope);

        return serviceBuilder;
    }

    /// <summary>
    /// Adds the services for the ImageSharp service.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <param name="scope">The optional service scope</param>
    /// <returns>The service collection</returns>
    public static IServiceCollection AddAeroImageSharp(this IServiceCollection services,
        ServiceLifetime scope = ServiceLifetime.Singleton)
    {
        services.Add(new ServiceDescriptor(typeof(IImageProcessor), typeof(ImageSharpProcessor), scope));

        return services;
    }
}
