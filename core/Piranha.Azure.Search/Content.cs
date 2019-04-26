/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Spatial;
using Piranha.Models;

namespace Piranha.Azure.Search
{
    [SerializePropertyNamesAsCamelCase]
    public class Content
    {
        [Key]
        public string ContentId { get; set; }

        [IsSearchable]
        public string Slug { get; set; }

        public string ContentType { get; set; }

        [IsSearchable]
        public string Title { get; set; }

        [IsFilterable]
        [IsSearchable]
        public string Category { get; set; }

        [IsFilterable]
        [IsSearchable]
        public IList<string> Tags { get; set; } = new List<string>();

        [IsSearchable]
        public string Body { get; set; }
    }
}