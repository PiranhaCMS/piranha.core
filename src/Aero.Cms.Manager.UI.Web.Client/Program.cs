using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Aero.Cms.Manager.UI.Shared.Services;
using Aero.Cms.Manager.UI.Web.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Add device-specific services used by the Aero.Cms.Manager.UI.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();

await builder.Build().RunAsync();
