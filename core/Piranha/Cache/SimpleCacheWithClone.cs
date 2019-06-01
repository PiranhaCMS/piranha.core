/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Piranha.Cache
{
    /// <summary>
    /// Simple in memory cache.
    /// </summary>
    public class SimpleCacheWithClone : SimpleCache
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="clone">If returned objects should be cloned</param>
        public SimpleCacheWithClone() : base(true) { }
    }
}
