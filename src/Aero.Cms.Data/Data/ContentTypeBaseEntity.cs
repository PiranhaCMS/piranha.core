

namespace Aero.Cms.Data.Data;

[Serializable]
public abstract class ContentTypeBase : Entity
{
    /// <summary>
    /// Gets/sets the CLR type of the content model.
    /// </summary>
    public string CLRType { get; set; }

    /// <summary>
    /// Gets/sets the JSON serialized body of the post type.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets/sets the last modification date.
    /// </summary>
    public DateTime LastModified { get; set; }
}
