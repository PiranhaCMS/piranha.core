/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Cache;

/// <summary>
/// The different cache levels available.
/// </summary>
public enum CacheLevel
{
    /// <summary>
    /// Nothing is cached
    /// </summary>
    None,
    /// <summary>
    /// Sites and Params are cached
    /// </summary>
    Minimal,
    /// <summary>
    /// Sites, Params, ContentTypes, PageTypes and PostTypes are cached
    /// </summary>
    Basic,
    /// <summary>
    /// Everything is cached
    /// </summary>
    Full
}
