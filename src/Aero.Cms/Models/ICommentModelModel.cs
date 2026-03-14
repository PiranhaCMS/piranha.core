

namespace Aero.Cms.Models;

/// <summary>
/// Interface for a content object that supports comments.
/// </summary>
public interface ICommentModel
{
    /// <summary>
    /// Gets/sets if comments should be enabled.
    /// </summary>
    /// <value></value>
    bool EnableComments { get; set; }
}
