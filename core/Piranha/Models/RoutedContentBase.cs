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
using Piranha.Extend.Fields;

namespace Piranha.Models;

/// <summary>
/// Abstract base class for templated content with a route.
/// </summary>
[Serializable]
public abstract class RoutedContentBase : ContentBase, IBlockContent, IMeta, ICommentModel
{
    /// <summary>
    /// Gets/sets the unique slug.
    /// </summary>
    [StringLength(128)]
    public string Slug { get; set; }

    /// <summary>
    /// Gets/sets the public permalink.
    /// </summary>
    public string Permalink { get; set; }

    /// <summary>
    /// Gets/sets the optional meta title.
    /// </summary>
    [StringLength(128)]
    public string MetaTitle { get; set; }

    /// <summary>
    /// Gets/sets the optional meta keywords.
    /// </summary>
    [StringLength(128)]
    public string MetaKeywords { get; set; }

    /// <summary>
    /// Gets/sets the optional meta description.
    /// </summary>
    [StringLength(256)]
    public string MetaDescription { get; set; }

    /// <summary>
    /// Gets/sets the meta index.
    /// </summary>
    public bool MetaIndex { get; set; } = true;

    /// <summary>
    /// Gets/sets the meta follow.
    /// </summary>
    public bool MetaFollow { get; set; } = true;

    /// <summary>
    /// Gets/sets the meta priority.
    /// </summary>
    public double MetaPriority { get; set; } = 0.5;

    /// <summary>
    /// Gets/sets the optional open graph title.
    /// </summary>
    [StringLength(128)]
    public string OgTitle { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph description.
    /// </summary>
    [StringLength(256)]
    public string OgDescription { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph image.
    /// </summary>
    public ImageField OgImage { get; set; } = new ImageField();

    /// <summary>
    /// Gets/sets the optional primary image.
    /// </summary>
    public ImageField PrimaryImage { get; set; } = new ImageField();

    /// <summary>
    /// Gets/sets the optional excerpt.
    /// </summary>
    public string Excerpt { get; set; }

    /// <summary>
    /// Gets/sets the optional route used by the middleware.
    /// </summary>
    [StringLength(256)]
    public string Route { get; set; }

    /// <summary>
    /// Gets/sets the optional redirect.
    /// </summary>
    [StringLength(256)]
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Gets/sets the redirect type.
    /// </summary>
    public RedirectType RedirectType { get; set; }

    /// <summary>
    /// Gets/sets the available blocks.
    /// </summary>
    public IList<Extend.Block> Blocks { get; set; } = new List<Extend.Block>();

    /// <summary>
    /// Gets/sets if comments should be enabled.
    /// </summary>
    /// <value></value>
    public bool EnableComments { get; set; }

    /// <summary>
    /// Gets/sets after how many days after publish date comments
    /// should be closed. A value of 0 means never.
    /// </summary>
    public int CloseCommentsAfterDays { get; set; }

    /// <summary>
    /// Gets/sets the comment count.
    /// </summary>
    public int CommentCount { get; set; }

    /// <summary>
    /// Checks if comments are open for this page.
    /// </summary>
    public bool IsCommentsOpen => EnableComments && Published.HasValue && (CloseCommentsAfterDays == 0 || Published.Value.AddDays(CloseCommentsAfterDays) > DateTime.Now);

    /// <summary>
    /// Gets/sets the published date.
    /// </summary>
    public DateTime? Published { get; set; }

    /// <summary>
    /// Checks of the current content is published.
    /// </summary>
    public bool IsPublished => Published.HasValue && Published.Value <= DateTime.Now;
}
