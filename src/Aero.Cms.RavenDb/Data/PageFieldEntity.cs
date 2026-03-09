

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class PageField : ContentFieldBase
{
    /// <summary>
    /// Gets/sets the page id.
    /// </summary>
    public string PageId { get; set; }

    /// <summary>
    /// Gets/sets the page.
    /// </summary>
    [JsonIgnore]
    public Page Page { get; set; }
}
