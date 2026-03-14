

namespace Aero.Cms.AspNetCore.Security;

/// <summary>
/// The options available for the security middleware.
/// </summary>
public sealed class SecurityOptions
{
    /// <summary>
    /// Gets/sets the login url.
    /// </summary>
    public string LoginUrl { get; set; } = "/login";
}
