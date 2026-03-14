

namespace Aero.Cms.Data.Data;

[Serializable]
public sealed class ContentGroup : Models.ContentGroup
{
    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
