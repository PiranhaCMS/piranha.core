

namespace Aero.Cms.Manager.Models.Content;

public class ContentTypeModel
{
    /// <summary>
    /// Gets/sets the unique id.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Gets/sets the title.
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Gets/sets the optional description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Gets/sets the url for adding a new content model for the type.
    /// </summary>
    public string AddUrl { get; set; }
}
