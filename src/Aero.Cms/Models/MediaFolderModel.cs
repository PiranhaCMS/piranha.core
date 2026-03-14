

using System.ComponentModel.DataAnnotations;

namespace Aero.Cms.Models;

[Serializable]
public class MediaFolder : Entity
{
    /// <summary>
    /// Gets/sets the optional parent id.
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Gets/sets the folder name.
    /// </summary>
    [Required]
    [StringLength(128)]
    public string Name { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    [StringLength(512)]
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }
}

[Serializable]
public class MediaFolderSimple : Entity
{
    /// <summary>
    /// Gets/sets the folder name.
    /// </summary>
    [Required]
    [StringLength(128)]
    public string Name { get; set; }
}
