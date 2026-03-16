

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Serializers;

/// <summary>
/// Serializer for media fields.
/// </summary>
public class MediaFieldSerializer : ISerializer
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is MediaField field)
        {
            return field.Id.ToString();
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        return new MediaField
        {
            Id = !string.IsNullOrEmpty(str) ? str : null
        };
    }
}
