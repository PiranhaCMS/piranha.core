

namespace Aero.Cms.Models;

/// <summary>
/// The different types of taxonomies available.
/// </summary>
[Serializable]
public enum TaxonomyType
{
    /// <summary>
    /// The type has not been specified.
    /// </summary>
    NotSet,
    /// <summary>
    /// The taxonomy is a category.
    /// </summary>
    Category,
    /// <summary>
    /// The taxonomy is a tag.
    /// </summary>
    Tag
}
