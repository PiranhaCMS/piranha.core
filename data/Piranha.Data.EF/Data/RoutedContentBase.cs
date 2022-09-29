/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Data;

[Serializable]
public abstract class RoutedContentBase<T> : ContentBase<T> where T : ContentFieldBase
{
    /// <summary>
    /// Gets/sets the unique slug.
    /// </summary>
    public string Slug { get; set; }

    /// <summary>
    /// Gets/sets the optional meta title.
    /// </summary>
    public string MetaTitle { get; set; }

    /// <summary>
    /// Gets/sets the optional meta keywords.
    /// </summary>
    public string MetaKeywords { get; set; }

    /// <summary>
    /// Gets/sets the optional meta description.
    /// </summary>
    public string MetaDescription { get; set; }

    /// <summary>
    /// Gets/sets the meta index.
    /// </summary>
    public bool? MetaIndex { get; set; }

    /// <summary>
    /// Gets/sets the meta follow.
    /// </summary>
    public bool? MetaFollow { get; set; }

    /// <summary>
    /// Gets/sets the meta priority.
    /// </summary>
    public double MetaPriority { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph title.
    /// </summary>
    public string OgTitle { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph description.
    /// </summary>
    public string OgDescription { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph image.
    /// </summary>
    public Guid OgImageId { get; set; }


    /// <summary>
    /// Gets/sets the optional route.
    /// </summary>
    public string Route { get; set; }

    /// <summary>
    /// Gets/sets the publishe date.
    /// </summary>
    public DateTime? Published { get; set; }
}
