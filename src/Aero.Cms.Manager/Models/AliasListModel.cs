

namespace Aero.Cms.Manager.Models;

/// <summary>
/// Alias model.
/// </summary>
public class AliasListModel
{
    /// <summary>
    /// A list item in the alias model.
    /// </summary>
    public class AliasItem
    {
        /// <summary>
        /// Gets/sets the optional id.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets/sets the site id.
        /// </summary>
        public string SiteId { get; set; }

        /// <summary>
        /// Gets/sets the alias url.
        /// </summary>
        public string AliasUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets if the redirect should be permanent.
        /// </summary>
        public bool IsPermanent { get; set; }
    }

    public class AliasSite
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    /// <summary>
    /// Gets/sets the current site id.
    /// </summary>
    public string SiteId { get; set; }

    /// <summary>
    /// Gets/sets the current site title.
    /// </summary>
    public string SiteTitle { get; set; }

    /// <summary>
    /// Gets/sets the available sites.
    /// </summary>
    public List<AliasSite> Sites { get; set; } = new List<AliasSite>();

    /// <summary>
    /// Gets/set the available items.
    /// </summary>
    public List<AliasItem> Items { get; set; } = new List<AliasItem>();

    /// <summary>
    /// Gets/sets the optional status message from the last operation.
    /// </summary>
    public StatusMessage Status { get; set; }
}
