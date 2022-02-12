/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Piranha.AspNetCore
{
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

        public void UseEndpoints(Action<IEndpointRouteBuilder> configuration)
        {
            Endpoints.Add(configuration);
        }
    }
}