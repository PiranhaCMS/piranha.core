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
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Slug { get; set; }

        [IsFilterable]
        public string ContentType { get; set; }

        [IsSearchable]
        [IsSortable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Title { get; set; }

        [IsFilterable]
        [IsSearchable]
        [IsSortable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Category { get; set; }

        [IsFilterable]
        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public IList<string> Tags { get; set; } = new List<string>();

        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Body { get; set; }
    }
}