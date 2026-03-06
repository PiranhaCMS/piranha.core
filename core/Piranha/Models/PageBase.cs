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
using System.Text.Json.Serialization;

namespace Piranha.Models;

/// <summary>
/// Base class for page models.
/// </summary>
[Serializable]
[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(typeof(DynamicPage), typeDiscriminator: "DynamicPage")]
[JsonDerivedType(typeof(PageInfo), typeDiscriminator: "PageInfo")]
public abstract class PageBase : RoutedContentBase
{
    /// <summary>
    /// Gets/sets the site id.
    /// </summary>
    public string SiteId { get; set; }

    /// <summary>
    /// Gets/sets the optional parent id.
    /// </summary>
    public string ParentId { get; set; }

    /// <summary>
    /// Gets/sets the sort order of the page in its hierarchical position.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets/sets the navigation title.
    /// </summary>
    [StringLength(128)]
    public string NavigationTitle { get; set; }

    /// <summary>
    /// Gets/sets if the page is hidden in the navigation.
    /// </summary>
    public bool IsHidden { get; set; }

    /// <summary>
    /// Gets/sets the id of the page this page is a copy of
    /// </summary>
    public string OriginalPageId { get; set; }

    /// <summary>
    /// Gets if this is the startpage of the site.
    /// </summary>
    public bool IsStartPage => string.IsNullOrEmpty(ParentId) && SortOrder == 0;
}
