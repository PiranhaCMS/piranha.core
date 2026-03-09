

using System.ComponentModel.DataAnnotations;

namespace Aero.Cms.Models;

[Serializable]
public abstract class MediaBase : Entity
{
    /// <summary>
    /// Gets/sets the optional folder id.
    /// </summary>
    public string? FolderId { get; set; }

    /// <summary>
    /// Gets/sets the media type.
    /// </summary>
    public MediaType Type { get; set; }

    /// <summary>
    /// Gets/sets the filename.
    /// </summary>
    [Required]
    [StringLength(128)]
    public string Filename { get; set; }

    /// <summary>
    /// Gets/sets the content type.
    /// </summary>
    [Required]
    [StringLength(256)]
    public string ContentType { get; set; }

    /// <summary>
    /// Gets/sets the optional title.
    /// </summary>
    [StringLength(128)]
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional alt text.
    /// </summary>
    [StringLength(128)]
    public string AltText { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    [StringLength(512)]
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the file size in bytes.
    /// </summary>
    public long Size { get; set; }

    /// <summary>
    /// Gets/sets the public url.
    /// </summary>
    public string PublicUrl { get; set; }

    /// <summary>
    /// Gets/sets the optional width. This only applies
    /// if the media asset is an image.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets/sets the optional height. This only applies
    /// if the media asset is an image.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
