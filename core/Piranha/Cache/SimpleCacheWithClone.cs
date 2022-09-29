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
/// Simple in memory cache.
/// </summary>
[NoCoverage]
public class SimpleCacheWithClone : SimpleCache
{
    /// <summary>
    /// Default constructor.
    /// </summary>
    public SimpleCacheWithClone() : base(true) { }
}
