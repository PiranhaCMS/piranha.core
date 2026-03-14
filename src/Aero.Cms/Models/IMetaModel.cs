

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Models;

public interface IMeta
{
    /// <summary>
    /// Gets/sets the title.
    /// </summary>
    string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional meta title.
    /// </summary>
    string MetaTitle { get; set; }

    /// <summary>
    /// Gets/sets the optional meta keywords.
    /// </summary>
    string MetaKeywords { get; set; }

    /// <summary>
    /// Gets/sets the optional meta description.
    /// </summary>
    string MetaDescription { get; set; }

            /// <summary>
    /// Gets/sets the meta index.
    /// </summary>
    bool MetaIndex { get; set; }

    /// <summary>
    /// Gets/sets the meta follow.
    /// </summary>
    bool MetaFollow { get; set; }

    /// <summary>
    /// Gets/sets the meta priority.
    /// </summary>
    double MetaPriority { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph title.
    /// </summary>
    string OgTitle { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph description.
    /// </summary>
    string OgDescription { get; set; }

    /// <summary>
    /// Gets/sets the optional open graph image.
    /// </summary>
    ImageField OgImage { get; set; }
}
