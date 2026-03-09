

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class PageComment : Comment
{
    /// <summary>
    /// Gets/sets the page id.
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// Gets/sets the page.
    /// </summary>
    public Page Page { get; set; }
}
