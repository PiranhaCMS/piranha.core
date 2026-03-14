

using System.Text.Json.Serialization;


namespace Aero.Cms.Extend.Fields;

/// <summary>
/// Base class for all SelectFields.
/// </summary>
public abstract class SelectFieldBase : IField
{
    /// <summary>
    /// Gets the type of the enum.
    /// </summary>
    [JsonIgnore]
    public abstract Type EnumType { get; }

    /// <summary>
    /// Gets/sets the value of the current enum value.
    /// </summary>
    [JsonIgnore]
    public abstract string EnumValue { get; set; }

    /// <summary>
    /// Gets the available items to choose from. Primarily used
    /// from the manager interface.
    /// </summary>
    [JsonIgnore]
    public abstract List<SelectFieldItem> Items { get; }

    /// <inheritdoc />
    public abstract string GetTitle();

    /// <summary>
    /// Initializes the field for client use.
    /// </summary>
    /// <param name="api">The current api</param>
    public abstract void Init(IApi api);
}
