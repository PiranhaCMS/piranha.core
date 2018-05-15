/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for basic content pages.
    /// </summary>
    public class Page<T> : GenericPage<T>, IPage where T : Page<T> { }
}