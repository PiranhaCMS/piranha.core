

namespace Aero.Cms.AspNetCore;

/// <summary>
/// The options available for the cms middleware components.
/// </summary>
public sealed class RoutingOptions
{
    /// <summary>
    /// Gets/sets if alias routing should be used.
    /// </summary>
    public bool UseAliasRouting { get; set; } = true;

    /// <summary>
    /// Gets/sets if archive routing should be used.
    /// </summary>
    public bool UseArchiveRouting { get; set; } = true;

    /// <summary>
    /// Gets/sets if page routing should be used.
    /// </summary>
    public bool UsePageRouting { get; set; } = true;

    /// <summary>
    /// Gets/sets if post routing should be used.
    /// </summary>
    public bool UsePostRouting { get; set; } = true;

    /// <summary>
    /// Gets/sets if site routing for multiple sites
    /// should be used.
    /// </summary>
    public bool UseSiteRouting { get; set; } = true;

    /// <summary>
    /// Gets/sets if sitemap routing should be used.
    /// </summary>
    public bool UseSitemapRouting { get; set; } = true;

    /// <summary>
    /// Gets/sets if startpage routing for empty URL's
    /// should be used.
    /// </summary>
    public bool UseStartpageRouting { get; set; } = true;
}
