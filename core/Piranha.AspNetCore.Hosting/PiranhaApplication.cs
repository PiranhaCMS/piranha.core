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

namespace Piranha.AspNetCore;

/// <summary>
/// Application builder for ASP.NET minimal hosting model
/// </summary>
public class PiranhaApplication : PiranhaApplicationBuilder
{
    /// <summary>
    /// Gets/sets the current Piranha Api.
    /// </summary>
    public IApi Api { get; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="app">The web application</param>
    /// <param name="api">The current api</param>
    public PiranhaApplication(WebApplication app, IApi api) : base(app)
    {
        Api = api;
    }
}
