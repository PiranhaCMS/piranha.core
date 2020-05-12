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
using System.Collections.Generic;

namespace Piranha.Manager.Models
{
    public class PostModalModel
    {
        public class PostModalItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Permalink { get; set; }
            public string Published { get; set; }
        }

        public class SiteItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
        }

        public class ArchiveItem
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Slug { get; set; }
        }

        public IEnumerable<SiteItem> Sites { get; set; } = new List<SiteItem>();
        public IEnumerable<ArchiveItem> Archives { get; set; } = new List<ArchiveItem>();
        public IEnumerable<PostModalItem> Posts { get; set; } = new List<PostModalItem>();

        public Guid SiteId { get; set; }
        public Guid ArchiveId { get; set; }

        public string SiteTitle { get; set; }
        public string ArchiveTitle { get; set; }
        public string ArchiveSlug { get; set; }
    }
}
