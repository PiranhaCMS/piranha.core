

namespace Aero.Cms.Models;

[Serializable]
public sealed class Media : MediaBase
{
    /// <summary>
    /// Gets/sets the user defined properties.
    /// </summary>
    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Gets/sets the available versions.
    /// </summary>
    public List<MediaVersion> Versions { get; set; } = new List<MediaVersion>();
}
