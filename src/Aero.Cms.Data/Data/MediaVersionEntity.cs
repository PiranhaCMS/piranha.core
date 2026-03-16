

using System.Text.Json.Serialization;

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class MediaVersion : Models.MediaVersion
{
    /// <summary>
    /// Gets/sets the id of the media this is
    /// a version of.
    /// </summary>
    public string MediaId { get; set; }

    /// <summary>
    /// Gets/sets the media this is a version of.
    /// </summary>
    [JsonIgnore]
    public Media Media { get; set; }
}
