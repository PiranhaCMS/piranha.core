

namespace Aero.Cms.RavenDb.Data;

[Serializable]
public sealed class MediaFolder : Models.MediaFolder
{
    /// <summary>
    /// Gets/sets the available media.
    /// </summary>
    public List<Media> Media { get; set; } = new List<Media>();
}
