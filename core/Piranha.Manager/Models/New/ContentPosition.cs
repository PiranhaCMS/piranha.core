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

namespace Piranha.Manager.Models
{
    public sealed class ContentPosition
    {
        public Guid SiteId { get; set; }
        public int SortOrder { get; set; }
        public bool IsHidden { get; set; }
    }
}