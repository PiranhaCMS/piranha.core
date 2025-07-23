/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.AspNetCore.Helpers;

/// <summary>
/// The request helper provides information regarding the
/// current request.
/// </summary>
public sealed class RequestHelper : IRequestHelper
{
    /// <summary>
    /// Gets/sets the current hostname.
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// Gets/sets the current port.
    /// </summary>
    public int? Port { get; set; }

    /// <summary>
    /// Gets/sets the current scheme.
    /// </summary>
    public string Scheme { get; set; }

    /// <summary>
    /// Gets/sets the requested raw url.
    /// </summary>
    public string Url { get; set; }
}
