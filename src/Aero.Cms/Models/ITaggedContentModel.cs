

namespace Aero.Cms.Models;

/// <summary>
/// Interface for content that can be tagged.
/// </summary>
public interface ITaggedContent
{
    /// <summary>
    /// Gets/sets the available tags.
    /// </summary>
    List<Taxonomy> Tags { get; set; }
}
