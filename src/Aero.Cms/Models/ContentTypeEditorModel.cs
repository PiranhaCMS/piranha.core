

namespace Aero.Cms.Models;

/// <summary>
/// Custom editor for a content type.
/// </summary>
[Serializable]
public sealed class ContentTypeEditor
{
    /// <summary>
    /// Gets/sets the editor component.
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// Gets/sets the optional icon css.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets/sets the title.
    /// </summary>
    public string Title { get; set; }
}
