/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.ComponentModel.DataAnnotations;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;

namespace Piranha.Azure.Search;

/// <summary>
/// Search index model for Azure Search.
/// </summary>
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
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.StandardLucene)]
    public string Slug { get; set; }

    /// <summary>
    /// Gets/sets the content type.
    /// </summary>
    [SimpleField(IsFilterable = true)]
    public string ContentType { get; set; }

    /// <summary>
    /// Gets/sets the main title.
    /// </summary>
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.StandardLucene, IsSortable = true)]
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional category.
    /// </summary>
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.StandardLucene, IsFilterable = true, IsSortable = true)]
    public string Category { get; set; }

    /// <summary>
    /// Gets/sets the optional tags.
    /// </summary>
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.StandardLucene, IsFilterable = true, IsFacetable = true)]
    public IList<string> Tags { get; set; } = new List<string>();

    /// <summary>
    /// Gets/sets the main body.
    /// </summary>
    [SearchableField(AnalyzerName = LexicalAnalyzerName.Values.StandardLucene)]
    public string Body { get; set; }
}
