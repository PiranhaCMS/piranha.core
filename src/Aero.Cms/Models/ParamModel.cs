

using System.ComponentModel.DataAnnotations;

namespace Aero.Cms.Models;

/// <summary>
/// String parameter.
/// </summary>
[Serializable]
public class Param : Entity
{
    /// <summary>
    /// Gets/sets the unique key.
    /// </summary>
    [Required]
    [StringLength(64)]
    public string Key { get; set; }

    /// <summary>
    /// Gets/sets the value.
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    [StringLength(255)]
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
