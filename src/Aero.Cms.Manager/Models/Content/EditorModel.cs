

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Edit model for custom editors.
/// </summary>
public class EditorModel
{
    /// <summary>
    /// Gets/sets the unique client id.
    /// </summary>
    public string Uid { get; set; } = "uid-" + Math.Abs(Snowflake.NewId().GetHashCode()).ToString();

    /// <summary>
    /// Gets/sets the editor component.
    /// </summary>
    public string Component { get; set; }

    /// <summary>
    /// Gets/sets the optional icon css.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// Gets/sets the name.
    /// </summary>
    public string Name { get; set; }
}
