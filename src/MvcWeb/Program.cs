using MvcWeb;
using Aero.Cms;
using Aero.Cms.AttributeBuilder;
using Aero.Cms.Manager.Editor;
using Aero.Cms.AspNetCore.Identity.Extensions;
using Aero.Cms.AspNetCore.Identity;
using Aero.Cms.RavenDb.Extensions;
using Aero.Local;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

var config = builder.Configuration;

var log = new LoggerConfiguration()
    .ReadFrom.Configuration(config)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "microbian-web")
    .WriteTo.Console()
    .WriteTo.File("logs/aero-.log", rollingInterval: RollingInterval.Day)
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(config)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "microbian-web")
        .WriteTo.Console()
        .WriteTo.File("logs/aero-.log", rollingInterval: RollingInterval.Day)
        ;
});


log.Information("Starting application");

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
        if (!app.Environment.IsEnvironment("Testing"))
        {
            log.Information("Seeding data");
            Seed.RunAsync(options.Api).GetAwaiter().GetResult();
            log.Information("Data seeded");
        }
    });

    log.Information("Starting application");
    await app.RunAsync();
    log.Information("Application stopped");
}
catch (Exception ex)
{
    log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
       Log.CloseAndFlush();
}
