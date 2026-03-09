

using System.ComponentModel.DataAnnotations;

namespace Aero.Cms.Models;

public class Language : Entity
{
    /// <summary>
    /// Gets/sets the display title.
    /// </summary>
    [Required]
    [StringLength(64)]
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional culture.
    /// </summary>
    public string Culture { get; set; }

    /// <summary>
    /// Gets/sets if this is the default language.
    /// </summary>
    public bool IsDefault { get; set; }
}
