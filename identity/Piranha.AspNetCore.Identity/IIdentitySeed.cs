/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.AspNetCore.Identity
{
    /// <summary>
    /// Interface for creating a data seed for the identity module.
    /// </summary>
    public interface IIdentitySeed
    {
        /// <summary>
        /// Create the seed data.
        /// </summary>
        Task CreateAsync();
    }
}