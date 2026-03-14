

namespace Aero.Cms.Models;

[Serializable]
public class MediaStructure : Structure<MediaStructure, MediaStructureItem>
{
    /// <summary>
    /// Gets/sets the number of media items in the root folder.
    /// </summary>
    public int MediaCount { get; set; }

    /// <summary>
    /// Gets/sets the total amount of media items in structure.
    /// </summary>
    public int TotalCount { get; set; }
}
