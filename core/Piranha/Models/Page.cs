/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Models
{
    /// <summary>
    /// Base class for basic content pages.
    /// </summary>
    [Serializable]
    public class Page<T> : GenericPage<T> where T : Page<T> { }
}