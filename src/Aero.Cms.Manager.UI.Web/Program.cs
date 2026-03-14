using Aero.Cms.Manager.UI.Web.Components;
using Aero.Cms.Manager.UI.Shared.Services;
using Aero.Cms.Manager.UI.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

// Add device-specific services used by the Aero.Cms.Manager.UI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();

//app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(Aero.Cms.Manager.UI.Shared._Imports).Assembly,
        typeof(Aero.Cms.Manager.UI.Web.Client._Imports).Assembly);

app.Run();
