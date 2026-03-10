

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Aero.AspNetCore;

/// <summary>
/// Application builder for simple startup.
/// </summary>
public class AeroApplicationBuilder
{
    /// <summary>
    /// The inner Application Builder.
    /// </summary>
    public readonly IApplicationBuilder Builder;

    /// <summary>
    /// The currently registered endpoint configurations.
    /// </summary>
    internal List<Action<IEndpointRouteBuilder>> Endpoints { get; } = new();

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="builder">The current application builder</param>
    public AeroApplicationBuilder(IApplicationBuilder builder)
    {
        Builder = builder;
    }

    /// <summary>
    /// Adds an endpoint configuration to the application builder. This
    /// can be called multiple times, and the endpoints will be added
    /// in the order they were added.
    /// </summary>
    /// <param name="configuration">The endpoint configuration</param>
    public void UseEndpoints(Action<IEndpointRouteBuilder> configuration)
    {
        Endpoints.Add(configuration);
    }
}
