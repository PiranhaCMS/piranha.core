

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Serializers;

/// <summary>
/// Serializer for image fields.
/// </summary>
public class ImageFieldSerializer : ISerializer
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is ImageField field)
        {
            return field.Id.ToString();
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        return new ImageField
        {
            Id = !string.IsNullOrEmpty(str) ? str : null
        };
    }
}
