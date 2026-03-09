

using Aero.Cms.Models;

namespace Aero.Cms.Extend.Fields;

/// <summary>
/// Field for referenncing a video asset.
/// </summary>
[FieldType(Name = "Video", Shorthand = "Video", Component = "video-field")]
public class VideoField : MediaFieldBase<VideoField>
{
    /// <summary>
    /// Implicit operator for converting a string id to a field.
    /// </summary>
    /// <param name="id">The id value</param>
    public static implicit operator VideoField(string id)
    {
        return new VideoField { Id = id };
    }

    /// <summary>
    /// Implicit operator for converting a Guid id to a field.
    /// </summary>
    /// <param name="guid">The guid value</param>
    public static implicit operator VideoField(Guid guid)
    {
        return new VideoField { Id = guid.ToString() };
    }

    /// <summary>
    /// Implicit operator for converting a media object to a field.
    /// </summary>
    /// <param name="media">The media object</param>
    public static implicit operator VideoField(Media media)
    {
        return new VideoField { Id = media.Id };
    }

    /// <summary>
    /// Impicit operator for converting the field to an url string.
    /// </summary>
    /// <param name="video">The video field</param>
    public static implicit operator string(VideoField video)
    {
        if (video.Media != null)
        {
            return video.Media.PublicUrl;
        }
        return "";
    }
}
