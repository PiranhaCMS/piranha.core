/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.WebApi;

public sealed class WebApiOptions
{
    /// <summary>
    /// Gets/sets if anonymous users should be able to access
    /// the api. The default value is false.
    /// </summary>
    public bool AllowAnonymousAccess { get; set; }
}
