using MvcWeb;
using Aero.Cms;
using Aero.Cms.AttributeBuilder;
using Aero.Cms.Manager.Editor;
using Aero.Cms.AspNetCore.Identity.Extensions;
using Aero.Cms.AspNetCore.Identity;
using Aero.Cms.RavenDb.Extensions;
using Aero.Local;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.AddAero(options =>
{
    /**
     * This will enable automatic reload of .cshtml
     * without restarting the application. However since
     * this adds a slight overhead it should not be
     * enabled in production.
     */
    options.AddRazorRuntimeCompilation = true;

    options.UseCms();
    options.UseManager();
    options.UseFileStorage(naming: FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseTinyMCE();
    options.UseMemoryCache();
    options.UseIdentityWithSeed<RavenIdentityDb>();

    // Use RavenDB 
    builder.Services.AddAeroStore();
    // Use RavenDB Identity
    // builder.Services.AddAeroRavenDbIdentity();

    /**
     * Here you can configure the different permissions
     * that you want to use for securing content in the
     * application.
    options.UseSecurity(o =>
    {
        o.UsePermission("WebUser", "Web User");
    });
     */

    /**
     * Here you can specify the login url for the front end
     * application. This does not affect the login url of
     * the manager interface.
    options.LoginUrl = "login";
     */
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

try
{
    app.UseAero(options =>
    {
        // Initialize Aero.Cms
        App.Init(options.Api);

        // Build content types
        new ContentTypeBuilder(options.Api)
            .AddAssembly(typeof(Program).Assembly)
            .Build()
            .DeleteOrphans();

        // Configure Tiny MCE
        EditorConfig.FromFile("editorconfig.json");

        options.UseManager();
        options.UseTinyMCE();

        // Seed data
        Seed.RunAsync(options.Api).GetAwaiter().GetResult();
    });

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}
