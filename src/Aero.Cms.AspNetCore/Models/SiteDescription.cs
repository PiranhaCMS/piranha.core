

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.AspNetCore.Models;

/// <summary>
/// Razor Page model for a single page.
/// </summary>
public class SiteDescription
{
    /// <summary>
    /// Gets/sets the current site title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the current site description.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets/sets the current site logo.
    /// </summary>
    public ImageField Logo { get; set; }
}
