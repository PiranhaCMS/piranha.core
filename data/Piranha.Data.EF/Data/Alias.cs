/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;

namespace Piranha.Data
{
    [Serializable]
    public sealed class Alias : Models.Alias
    {
        /// <summary>
        /// Gets/sets the site this alias is for.
        /// </summary>
        /// <returns></returns>
        public Site Site { get; set; }
    }
}