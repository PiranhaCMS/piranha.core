/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Builder;

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
        /// Default constructor.
        /// </summary>
        /// <param name="builder">The current application builder</param>
        public PiranhaApplicationBuilder(IApplicationBuilder builder)
        {
            Builder = builder;
        }
    }
}