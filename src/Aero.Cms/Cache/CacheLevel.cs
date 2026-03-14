

namespace Aero.Cms.Cache;

/// <summary>
/// The different cache levels available.
/// </summary>
public enum CacheLevel
{
    /// <summary>
    /// Nothing is cached
    /// </summary>
    None,
    /// <summary>
    /// Sites and Params are cached
    /// </summary>
    Minimal,
    /// <summary>
    /// Sites, Params, ContentTypes, PageTypes and PostTypes are cached
    /// </summary>
    Basic,
    /// <summary>
    /// Everything is cached
    /// </summary>
    Full
}
