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
    // Nothing is stored cached
    None,
    // Sites & Params are cached
    Minimal,
    // Sites, Params, PageTypes & PostTypes are cached
    Basic,
    // Everything is cached
    Full
}
