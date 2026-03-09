

namespace Aero.Cms.AspNetCore.Identity;

/// <summary>
/// Interface for creating a data seed for the identity module.
/// </summary>
public interface IIdentitySeed
{
    /// <summary>
    /// Create the seed data.
    /// </summary>
    Task CreateAsync();
}
