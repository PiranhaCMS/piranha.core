

namespace Aero.AspNetCore.Http;

/// <summary>
/// Caching information about a content model.
/// </summary>
public sealed class HttpCacheInfo
{
    /// <summary>
    /// Gets/sets the entity tag.
    /// </summary>
    public string EntityTag { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime? LastModified { get; set; }
}
