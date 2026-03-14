

namespace Aero.Cms.Extend.Fields;

/// <summary>
/// An available item to choose from for a SelectField.
/// </summary>
public class SelectFieldItem
{
    /// <summary>
    /// Gets/sets the display title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the enum value.
    /// </summary>
    public Enum Value { get; set; }
}
