using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AttributeBuilder;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;
using Piranha.Services; // Adicionar para o IWorkflowService

var builder = WebApplication.CreateBuilder(args);

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

    var connectionString = builder.Configuration.GetConnectionString("piranha");
    options.UseEF<SQLiteDb>(db => db.UseSqlite(connectionString));
    options.UseIdentityWithSeed<IdentitySQLiteDb>(db => db.UseSqlite(connectionString));

    /**
     * Here you can configure the different permissions
     * that you want to use for securing content in the
     * application.
     */
    options.UseSecurity(o =>
    {
        // Permissões existentes
        o.UsePermission("WebUser", "Web User");

        // Novas permissões para Workflow
        o.UsePermission("PiranhaReviewer", "Workflow Reviewer");
        o.UsePermission("PiranhaLegalTeam", "Legal Team Reviewer");
    });

    /**
     * Here you can specify the login url for the front end
     * application. This does not affect the login url of
     * the manager interface.
    options.LoginUrl = "login";
     */
});

// Adicionar o serviço de Workflow
builder.Services.AddScoped<IWorkflowService, WorkflowService>();

// Configurar políticas de autorização
builder.Services.AddAuthorization(options =>
{
    // Política principal do Manager (mais permissiva para teste)
    options.AddPolicy(
        "PiranhaManager",
        policy =>
        {
            policy.RequireAuthenticatedUser();
        }
    );

    // Políticas específicas para Workflow
    options.AddPolicy(
        "WorkflowReviewer",
        policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("Permission", "PiranhaReviewer");
        }
    );

    options.AddPolicy(
        "WorkflowLegalTeam",
        policy =>
        {
            policy.RequireAuthenticatedUser();
            policy.RequireClaim("Permission", "PiranhaLegalTeam");
        }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

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
    options.UseIdentity();
});

app.Run();
