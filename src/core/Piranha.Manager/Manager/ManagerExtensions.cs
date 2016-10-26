using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Razor;

public static class ManagerExtensions
{
    public static IServiceCollection AddPiranhaManager(this IServiceCollection services) {
        var assembly = typeof(ManagerExtensions).GetTypeInfo().Assembly;
        var provider = new EmbeddedFileProvider(assembly);

        //Add the file provider to the Razor view engine
        services.Configure<RazorViewEngineOptions>(options =>
        {
            options.FileProviders.Add(provider);
        });
        return services;
    }
}
