/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Identity;

namespace Piranha.AspNetCore.Identity.Data
{
    /// <summary>
    /// The application user.
    /// </summary>
    public sealed class User : IdentityUser<Guid>
    {
    }
}