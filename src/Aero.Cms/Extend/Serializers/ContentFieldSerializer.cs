

using Aero.Cms.Extend.Fields;

namespace Aero.Cms.Extend.Serializers;

/// <summary>
/// Serializer for content fields.
/// </summary>
public class ContentFieldSerializer : ISerializer
{
    /// <inheritdoc />
    public string Serialize(object obj)
    {
        if (obj is Fields.ContentField field)
        {
            return field.Id.ToString();
        }
        throw new ArgumentException("The given object doesn't match the serialization type");
    }

    /// <inheritdoc />
    public object Deserialize(string str)
    {
        return new ContentField
        {
            Id = !string.IsNullOrEmpty(str) ? str : null
        };
    }
}
