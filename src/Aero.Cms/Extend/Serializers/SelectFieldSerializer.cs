

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Serializers;

/// <summary>
/// Serializer for select fields.
/// </summary>
/// <typeparam name="T">The field type</typeparam>
public class SelectFieldSerializer<T> : ISerializer where T : Fields.SelectFieldBase
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is SelectFieldBase field)
        {
            return field.EnumValue;
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        var field = Activator.CreateInstance<T>();

        if (!string.IsNullOrWhiteSpace(str))
        {
            try
            {
                field.EnumValue = str;
            }
            catch
            {
                // Let's not throw an exception, let's just
                // return a new empty field.
                field.EnumValue = null;
            }
        }
        return field;
    }
}
