

namespace Aero.Cms.Extend.Fields;

/// <summary>
/// Field for a string value that can't be edited.
/// </summary>
[FieldType(Name = "Readonly", Shorthand = "Readonly", Component = "readonly-field")]
public class ReadonlyField : SimpleField<string>
{
    /// <summary>
    /// Implicit operator for converting a string to a field.
    /// </summary>
    /// <param name="str">The string value</param>
    public static implicit operator ReadonlyField(string str)
    {
        return new ReadonlyField { Value = str };
    }

    /// <summary>
    /// Implicitly converts the readonly field to a string.
    /// </summary>
    /// <param name="field">The field</param>
    public static implicit operator string(ReadonlyField field)
    {
        return field.Value;
    }
}
