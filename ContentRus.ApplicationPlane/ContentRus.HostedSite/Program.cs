using Microsoft.EntityFrameworkCore;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.MySQL;
using Piranha.Data.EF.MySql;
using Piranha.Manager.Editor;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Piranha.AspNetCore.Http;

using OpenTelemetry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Exporter;

using System.Diagnostics;


var builder = WebApplication.CreateBuilder(args);

// Add OpenTelemetry services
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource
        .AddService("contentrus-hosted-site")
        .AddAttributes(new Dictionary<string, object>
        {
            ["deployment.environment"] = builder.Environment.EnvironmentName
        }))
    .WithMetrics(metrics => metrics
        .AddRuntimeInstrumentation()
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddOtlpExporter(options => {
            options.Endpoint = new Uri(builder.Configuration["Telemetry:Endpoint"] ?? 
                "http://simplest-collector.tcommon.svc.cluster.local:4317");
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
        .AddConsoleExporter())
    .WithTracing(tracing => tracing
        .SetResourceBuilder(ResourceBuilder.CreateDefault()
            .AddService(builder.Environment.ApplicationName))
        .SetSampler(new AlwaysOnSampler())
        .AddEntityFrameworkCoreInstrumentation()
        .AddAspNetCoreInstrumentation(options => {
        // Enable W3C trace context propagation
            options.EnrichWithHttpRequest = (activity, httpRequest) => {
                activity.SetTag("http.request.headers", string.Join(",", httpRequest.Headers.Keys));
            };
            options.RecordException = true;
        })
        .AddHttpClientInstrumentation(options => {
            // Ensure headers are propagated on outgoing requests
            options.EnrichWithHttpRequestMessage = (activity, request) => {
                activity.SetTag("http.request.uri", request.RequestUri.ToString());
            };
            options.RecordException = true;
        })
        .AddSqlClientInstrumentation(options =>
        {
            options.SetDbStatementForText = true;
            options.RecordException = true;
        })
        .AddOtlpExporter(options => {
            options.Endpoint = new Uri(builder.Configuration["Telemetry:Endpoint"] ?? 
                "http://simplest-collector.tcommon.svc.cluster.local:4317");
            options.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.Grpc;
        })
        .AddConsoleExporter());


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

    options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
    options.UseImageSharp();
    options.UseMemoryCache();

    var connectionString = builder.Configuration.GetConnectionString("piranha");
    options.UseEF<MySqlDb>(db =>
        db.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    // options.UseIdentityWithSeed<IdentityMySQLDb>(db =>
    //     db.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);


app.UsePiranha(options =>
{
    // Initialize Piranha
    App.Init(options.Api);

    // Build content types
    new ContentTypeBuilder(options.Api)
        .AddAssembly(typeof(Program).Assembly)
        .Build()
        .DeleteOrphans();

    // options.UseIdentity();
});

app.Run();