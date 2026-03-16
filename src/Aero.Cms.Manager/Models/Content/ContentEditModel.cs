

namespace Aero.Cms.Manager.Models.Content;

/// <summary>
/// Content edit model.
/// </summary>
public abstract class ContentEditModel : AsyncResult, IEntity
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public string Id { get; set; } = Snowflake.NewId();

    /// <summary>
    /// Gets/sets the content type id.
    /// </summary>
    public string TypeId { get; set; }

    /// <summary>
    /// Gets/sets the mandatory title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets if blocks should be used.
    /// </summary>
    public bool UseBlocks { get; set; } = true;

    /// <summary>
    /// Gets/sets if the content is scheduled.
    /// </summary>
    public bool IsScheduled { get; set; }

    /// <summary>
    /// Gets/sets the available blocks.
    /// </summary>
    public List<BlockModel> Blocks { get; set; } = new List<BlockModel>();

    /// <summary>
    /// Gets/sets the available regions.
    /// </summary>
    public List<RegionModel> Regions { get; set; } = new List<RegionModel>();

    /// <summary>
    /// Gets/sets the available custom editors.
    /// </summary>
    public List<EditorModel> Editors { get; set; } = new List<EditorModel>();
}
