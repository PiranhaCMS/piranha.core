

using Microsoft.AspNetCore.Http;

namespace Aero.Cms.Manager.Models;

/// <summary>
/// Model for uploading a new media asset.
/// </summary>
public class MediaUploadModel
{
    /// <summary>
    /// Gets/sets the optional id.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets/sets the parent id.
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Gets/sets the uploaded file.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<IFormFile> Uploads { get; set; } = new List<IFormFile>();
}
