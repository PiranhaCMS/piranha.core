/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Manager.Models.Content;
using Piranha.Models;

namespace Piranha.Manager.Models;

/// <summary>
/// Content list model.
/// </summary>
public class ContentListModel
{
    public class ContentItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string TypeId { get; set; }
        public string Modified { get; set; }
        public string Status { get; set; }
        public string EditUrl { get; set; }
    }

    public ContentGroup Group { get; set; }
    public IEnumerable<ContentGroup> Groups { get; set; } = new List<ContentGroup>();
    public IEnumerable<ContentItem> Items { get; set; } = new List<ContentItem>();
    public IList<ContentTypeModel> Types { get; set; } = new List<ContentTypeModel>();
    public StatusMessage Status { get; set; }
}
