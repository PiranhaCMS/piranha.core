

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Serializers;

/// <summary>
/// Serializer for date fields.
/// </summary>
public class DateFieldSerializer : ISerializer
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is DateField field)
        {
            if (field.Value.HasValue)
            {
                return field.Value.Value.ToString("yyyy-MM-dd");
            }
            return null;
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        var field = new DateField();

        if (!string.IsNullOrWhiteSpace(str))
        {
            try
            {
                field.Value = DateTime.Parse(str);
            }
            catch
            {
                // Let's not throw an exception, let's just
                // return a new empty field.
                field.Value = null;
            }
        }
        return field;
    }
}
