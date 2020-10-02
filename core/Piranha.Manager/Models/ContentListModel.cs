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
using Piranha.Manager.Models.Content;
using Piranha.Models;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Content list model.
    /// </summary>
    public class ContentListModel
    {
        public ContentGroup Group { get; set; }
        public IEnumerable<ContentInfo> Items { get; set; } = new List<ContentInfo>();
        public IList<ContentTypeModel> Types { get; set; } = new List<ContentTypeModel>();
        public StatusMessage Status { get; set; }
    }
}