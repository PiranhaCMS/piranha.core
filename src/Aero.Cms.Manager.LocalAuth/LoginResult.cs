

namespace Aero.Cms.Manager.LocalAuth;

/// <summary>
/// The different results a login can have.
/// </summary>
public enum LoginResult
{
    /// <summary>
    /// The login succeeded.
    /// </summary>
    Succeeded,
    /// <summary>
    /// The login failed.
    /// </summary>
    Failed,
    /// <summary>
    /// The user account is locked.
    /// </summary>
    Locked
}