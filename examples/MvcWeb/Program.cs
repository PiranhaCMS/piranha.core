using Aero.Identity;

using MvcWeb;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.Data.RavenDb;
using Piranha.Data.RavenDb.Extensions;
using Piranha.Manager.Editor;
using Raven.Client.Documents;

var builder = WebApplication.CreateBuilder(args);

// Configure RavenDB
var ravenStore = new DocumentStore
{
    Urls = [builder.Configuration.GetValue<string>("RavenDb:Url") ?? "http://localhost:8080"],
    Database = builder.Configuration.GetValue<string>("RavenDb:Database") ?? "piranha"
}.Initialize();

builder.Services.AddSingleton(ravenStore);
builder.Services.AddScoped(s => s.GetRequiredService<IDocumentStore>().OpenAsyncSession());
builder.AddPiranha(options =>
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
    
    options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseTinyMCE();
    options.UseMemoryCache();

    // Use RavenDB 
    builder.Services.AddPiranhaStore<DbRaven>();
    // Use RavenDB Identity
    builder.Services.AddPiranhaRavenDbIdentity();

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
    app.UsePiranha(options =>
    {
        // Initialize Piranha
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
