/*
 * Copyright (c) .NET Foundation and Contributors
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
    /// <summary>
    /// Search index model for Azure Search.
    /// </summary>
    [SerializePropertyNamesAsCamelCase]
    public class Content
    {
        /// <summary>
        /// Gets/sets the unique content id.
        /// </summary>
        [Key]
        public string ContentId { get; set; }

        /// <summary>
        /// Gets/sets the content slug.
        /// </summary>
        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Slug { get; set; }

        /// <summary>
        /// Gets/sets the content type.
        /// </summary>
        [IsFilterable]
        public string ContentType { get; set; }

        /// <summary>
        /// Gets/sets the main title.
        /// </summary>
        [IsSearchable]
        [IsSortable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the optional category.
        /// </summary>
        [IsFilterable]
        [IsSearchable]
        [IsSortable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Category { get; set; }

        /// <summary>
        /// Gets/sets the optional tags.
        /// </summary>
        [IsFilterable]
        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public IList<string> Tags { get; set; } = new List<string>();

        /// <summary>
        /// Gets/sets the main body.
        /// </summary>
        [IsSearchable]
        [Analyzer(AnalyzerName.AsString.StandardLucene)]
        public string Body { get; set; }
    }
}