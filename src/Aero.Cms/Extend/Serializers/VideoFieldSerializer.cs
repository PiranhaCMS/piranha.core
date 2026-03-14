

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Serializers;

/// <summary>
/// Serializer for video fields.
/// </summary>
public class VideoFieldSerializer : ISerializer
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is VideoField field)
        {
            return field.Id.ToString();
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        return new VideoField
        {
            Id = !string.IsNullOrEmpty(str) ? str : null
        };
    }
}
