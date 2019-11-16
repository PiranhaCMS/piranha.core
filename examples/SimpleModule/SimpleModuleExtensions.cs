using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Piranha;
using SimpleModule;

public static class SimpleModuleExtensions
{
    public static IServiceCollection AddSimpleModule(this IServiceCollection services)
    {
        App.Modules.Register<Module>();

        return services;
    }

    public static IApplicationBuilder UseSimpleModule(this IApplicationBuilder builder)
    {
        // Manager resources
        App.Modules.Get<Piranha.Manager.Module>().Scripts.Add("~/manager/simplemodule/js/header-block.js");

        // Add the embedded resources
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(SimpleModuleExtensions).Assembly, "SimpleModule.assets.dist"),
            RequestPath = "/manager/simplemodule"
        });

    }
}