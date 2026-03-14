

namespace Aero.Cms.Models;

[Serializable]
public class MediaStructureItem : StructureItem<MediaStructure, MediaStructureItem>
{
    /// <summary>
    /// Gets/sets the folder name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets/sets the number of child folders in the folder.
    /// </summary>
    public int FolderCount { get; set; }

    /// <summary>
    /// Gets/sets the number of media items in the folder.
    /// </summary>
    public int MediaCount { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public MediaStructureItem()
    {
        Items = new MediaStructure();
    }
}
