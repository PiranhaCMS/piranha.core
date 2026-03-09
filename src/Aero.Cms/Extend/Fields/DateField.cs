

namespace Aero.Cms.Extend.Fields;

/// <summary>
/// Field for a datetime value.
/// </summary>
[FieldType(Name = "Date", Shorthand = "Date", Component = "date-field")]
public class DateField : SimpleField<DateTime?>
{
    /// <summary>
    /// Converts the given datetime to a field.
    /// </summary>
    /// <param name="date">The datetime</param>
    public static implicit operator DateField(DateTime date)
    {
        return new DateField { Value = date };
    }
}
