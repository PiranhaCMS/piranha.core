/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Piranha.AspNetCore;

/// <summary>
/// Application builder for simple startup.
/// </summary>
public class PiranhaApplicationBuilder
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
    public PiranhaApplicationBuilder(IApplicationBuilder builder)
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
