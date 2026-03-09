

namespace Aero.Cms.Manager.Models;

/// <summary>
/// Site list model.
/// </summary>
public class SiteListModel
{
    /// <summary>
    /// Gets/sets the currently selected site id.
    /// </summary>
    public required string SiteId { get; set; } = Snowflake.NewId();

    /// <summary>
    /// Gets/sets the title of the currently selected site.
    /// </summary>
    public string SiteTitle { get; set; }

    /// <summary>
    /// Gets/sets the available sites.
    /// </summary>
    public List<PageListModel.PageSite> Sites { get; set; } = new List<PageListModel.PageSite>();

    /// <summary>
    /// Gets/sets the items in the currently selected site.
    /// </summary>
    public List<PageListModel.PageItem> Items { get; set; } = new List<PageListModel.PageItem>();
}
