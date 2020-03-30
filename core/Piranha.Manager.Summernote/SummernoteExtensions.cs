/*
 * Copyright (c) .NET Foundation and Contributors
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
using Piranha;
using Piranha.Manager.Editor;

public static class SummernoteExtensions
{
    /// <summary>
    /// Adds the Summernote editor module.
    /// </summary>
    /// <param name="services">The current service collection</param>
    /// <returns>The services</returns>
    public static IServiceCollection AddPiranhaSummernote(this IServiceCollection services)
    {
        // Add the manager module
        Piranha.App.Modules.Register<Piranha.Manager.Summernote.Module>();

        // Return the service collection
        return services;
    }

    /// <summary>
    /// Uses the Tiny MCE editor module.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    /// <param name="withCodeMirrorDefaults"></param>
    /// <returns>The builder</returns>
    public static IApplicationBuilder UsePiranhaSummernote(this IApplicationBuilder builder, bool withCodeMirrorDefaults = true)
    {
        //
        // Register the editor scripts.
        //
        EditorScripts.MainScriptUrl = "~/manager/summernote/summernote.min.js";
        EditorScripts.EditorScriptUrl = "~/manager/summernote/piranha.summernote.min.js";

        //
        // Register styles
        //
        App.Modules.Get<Piranha.Manager.Module>().Styles.Add("~/manager/summernote/piranha.summernote.min.css");

        //
        // Register partials
        //
        App.Modules.Get<Piranha.Manager.Module>().Partials.Add("~/Areas/Manager/Shared/Partial/_SummernoteLink.cshtml");

        //
        // Register Codemirror styles and scripts
        //
        // ReSharper disable once InvertIf
        if (withCodeMirrorDefaults)
        {
            App.Modules.Get<Piranha.Manager.Module>().Styles.Add("~/manager/summernote/codemirror.css");
            App.Modules.Get<Piranha.Manager.Module>().Styles.Add("~/manager/summernote/show-hint.css");
            App.Modules.Get<Piranha.Manager.Module>().Scripts.Add("~/manager/summernote/codemirror.js");
            App.Modules.Get<Piranha.Manager.Module>().Scripts.Add("~/manager/summernote/xml.js");
            App.Modules.Get<Piranha.Manager.Module>().Scripts.Add("~/manager/summernote/show-hint.js");
            App.Modules.Get<Piranha.Manager.Module>().Scripts.Add("~/manager/summernote/xml-hint.js");
            App.Modules.Get<Piranha.Manager.Module>().Scripts.Add("~/manager/summernote/html-hint.js");
        }

        //
        // Add the embedded resources
        //
        return builder.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new EmbeddedFileProvider(typeof(SummernoteExtensions).Assembly, "Piranha.Manager.Summernote.assets.dist"),
            RequestPath = "/manager/summernote"
        });
    }
}
