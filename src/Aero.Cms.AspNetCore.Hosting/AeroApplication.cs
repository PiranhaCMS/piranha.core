

using Aero.Cms;
using Microsoft.AspNetCore.Builder;

namespace Aero.AspNetCore;

/// <summary>
/// Application builder for ASP.NET minimal hosting model
/// </summary>
public class AeroApplication : AeroApplicationBuilder
{
    /// <summary>
    /// Gets/sets the current Aero.Cms Api.
    /// </summary>
    public IApi Api { get; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="api">The current api</param>
    public AeroApplication(WebApplication app, IApi api) : base(app)
    {
        Api = api;
    }
}
