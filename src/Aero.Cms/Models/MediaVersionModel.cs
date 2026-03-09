

namespace Aero.Cms.Models;

[Serializable]
public class MediaVersion : Entity
{
    /// <summary>
    /// Gets/sets the file size in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets/sets the width.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets/sets the optional height.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets/sets the file extension of the generated version.
    /// </summary>
    public string FileExtension { get; set; }
}
