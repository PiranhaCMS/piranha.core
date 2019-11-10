/*
 * Copyright (c) 2016-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Piranha.Manager.Editor;

public static class TinyMCEExtensions
{
    /// <summary>
    /// Adds the Tiny MCE editor module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaTinyMCE(this IServiceCollection services) {
        // Add the manager module
        Piranha.App.Modules.Register<Piranha.Manager.TinyMCE.Module>();

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the Tiny MCE editor module.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaTinyMCE(this IApplicationBuilder builder) {
        //
        // Register the editor scripts.
        //
        EditorScripts.MainScriptUrl = "~/manager/tiny/tinymce/tinymce.min.js";
        EditorScripts.EditorScriptUrl = "~/manager/tiny/piranha.editor.js";

        //
        // Add the embedded resources
        //
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(TinyMCEExtensions).Assembly, "Piranha.Manager.TinyMCE.assets"),
            RequestPath = "/manager/tiny"
        });
    }
}
