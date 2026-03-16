

namespace Aero.Cms.Extend.Fields;

/// <summary>
/// Field for an integer value.
/// </summary>
[FieldType(Name = "Number", Shorthand = "Number", Component = "number-field")]
public class NumberField : SimpleField<int?>
{
    /// <summary>
    /// Implicit operator for converting a int to a field.
    /// </summary>
    /// <param name="number">The integer value</param>
    public static implicit operator NumberField(int number)
    {
        return new NumberField { Value = number };
    }

    /// <summary>
    /// Implicitly converts the Number field to a int.
    /// </summary>
    /// <param name="field">The field</param>
    public static implicit operator int? (NumberField field)
    {
        return field.Value;
    }
}
