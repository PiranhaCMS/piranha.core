/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Models
{
    /// <summary>
    /// Base class for basic content pages.
    /// </summary>
    public class Page<T> : GenericPage<T> where T : Page<T> { }
}