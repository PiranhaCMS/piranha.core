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
    /// Simple post class for querying large sets of
    /// data without loading regions or blocks.
    /// </summary>
    [Serializable]
    public class PageInfo : PageBase, IContentInfo { }
}
