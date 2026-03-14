

using System.ComponentModel.DataAnnotations;

namespace Aero.Cms.Models;

[Serializable]
public class Alias : Entity
{
    /// <summary>
    /// Gets/sets the id of the site this alias is for.
    /// </summary>
    public string SiteId { get; set; }

    /// <summary>
    /// Gets/sets the alias url.
    /// </summary>
    [Required]
    [StringLength(256)]
    public string AliasUrl { get; set; }

    /// <summary>
    /// Gets/sets the url of the redirect.
    /// </summary>
    [Required]
    [StringLength(256)]
    public string RedirectUrl { get; set; }

    /// <summary>
    /// Gets/sets if this is a permanent or temporary
    /// redirect.
    /// </summary>
    public RedirectType Type { get; set; } = RedirectType.Temporary;

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
