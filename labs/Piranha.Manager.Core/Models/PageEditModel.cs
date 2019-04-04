/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using Piranha.Extend;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Page edit model.
    /// </summary>
    public class PageEditModel : Content.ContentEditModel
    {
        public Guid SiteId { get; set; }
        public Guid? ParentId { get; set; }
        public int SortOrder { get; set; }
        public string NavigationTitle { get; set; }
        public string Slug { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string Published { get; set; }
    }
}