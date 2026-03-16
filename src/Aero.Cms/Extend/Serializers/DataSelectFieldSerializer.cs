

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Serializers;

/// <summary>
/// Serializer for data select fields.
/// </summary>
/// <typeparam name="T">The field type</typeparam>
public class DataSelectFieldSerializer<T> : ISerializer where T : Fields.DataSelectFieldBase
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is DataSelectFieldBase field)
        {
            return field.Id;
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
                field.Id = str;
            }
            catch
            {
                // Let's not throw an exception, let's just
                // return a new empty field.
                field.Id = null;
            }
        }
        return field;
    }
}
