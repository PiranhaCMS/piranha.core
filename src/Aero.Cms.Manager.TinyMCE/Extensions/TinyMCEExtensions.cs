

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Aero.Cms.Manager.Editor;

/// <summary>
/// Extension class for adding TinyMCE to the web application.
/// </summary>
public static class TinyMCEExtensions
{
    /// <summary>
    /// Adds the Tiny MCE editor module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddAeroTinyMCE(this IServiceCollection services) {
        // Add the manager module
        Aero.Cms.App.Modules.Register<Aero.Cms.Manager.TinyMCE.Module>();

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the Tiny MCE editor module.
    /// </summary>
    /// <param name="builder">The application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UseAeroTinyMCE(this IApplicationBuilder builder) {
        //
        // Register the editor scripts.
        //
        EditorScripts.MainScriptUrl = "~/manager/tiny/tinymce/tinymce.min.js";
        EditorScripts.EditorScriptUrl = "~/manager/tiny/Aero.editor.js";

        //
        // Add the embedded resources
        //
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(TinyMCEExtensions).Assembly, "Aero.Cms.Manager.TinyMCE.assets"),
            RequestPath = "/manager/tiny"
        });
    }
}
