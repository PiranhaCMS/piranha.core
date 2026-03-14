

namespace Aero.Cms;

/// <summary>
/// Abstract base class for database entities.
/// </summary>
public abstract class Entity : IEntity
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public string? Id { get; set; } = null;
}
