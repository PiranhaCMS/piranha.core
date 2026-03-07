using System.ComponentModel.DataAnnotations;

namespace Piranha.Models;

/// <summary>
/// Base class for post models.
/// </summary>
[Serializable]
public abstract class PostBase : RoutedContentBase, ICategorizedContent, ITaggedContent
{
    /// <summary>
    /// Gets/sets the blog page id.
    /// </summary>
    [Required]
    public string BlogId { get; set; }

    //[Required] - // todo - mark required once we get tests passing
    public string SiteId { get; set; }

    /// <summary>
    /// Gets/sets the category.
    /// </summary>
    [Required]
    public Taxonomy Category { get; set; }

    /// <summary>
    /// Gets/sets the available tags.
    /// </summary>
    public List<Taxonomy> Tags { get; set; } = new List<Taxonomy>();
}
