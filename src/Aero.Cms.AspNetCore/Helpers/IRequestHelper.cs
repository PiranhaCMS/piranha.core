

namespace Aero.Cms.AspNetCore.Helpers;

/// <summary>
/// The request helper provides information regarding the
/// current request.
/// </summary>
public interface IRequestHelper
{
    /// <summary>
    /// Gets/sets the current hostname.
    /// </summary>
    string Host { get; set; }

    /// <summary>
    /// Gets/sets the current port.
    /// </summary>
    int? Port { get; set; }

    /// <summary>
    /// Gets/sets the current scheme.
    /// </summary>
    string Scheme { get; set; }

    /// <summary>
    /// Gets/sets the requested raw url.
    /// </summary>
    string Url { get; set; }
}
