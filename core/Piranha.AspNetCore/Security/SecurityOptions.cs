/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.AspNetCore.Security;

/// <summary>
/// The options available for the security middleware.
/// </summary>
public sealed class SecurityOptions
{
    /// <summary>
    /// Gets/sets the login url.
    /// </summary>
    public string LoginUrl { get; set; } = "/login";
}
