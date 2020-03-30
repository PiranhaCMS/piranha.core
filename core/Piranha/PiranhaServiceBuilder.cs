/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.DependencyInjection;

namespace Piranha
{
    /// <summary>
    /// Service builder for simple application startup.
    /// </summary>
    public class PiranhaServiceBuilder : PiranhaRouteConfig
    {
        /// <summary>
        /// The inner Service collection.
        /// </summary>
        public readonly IServiceCollection Services;

        /// <summary>
        /// Gets/sets if runtime compilation should be enabled.
        /// </summary>
        public bool AddRazorRuntimeCompilation { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="services">The current service collection</param>
        public PiranhaServiceBuilder(IServiceCollection services)
        {
            Services = services;
        }
    }
}