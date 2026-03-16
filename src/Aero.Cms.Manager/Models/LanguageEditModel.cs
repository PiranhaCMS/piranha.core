

using Aero.Cms.Models;

namespace Aero.Cms.Manager.Models;

/// <summary>
/// Language modal edit model.
/// </summary>
public class LanguageEditModel
{
    /// <summary>
    /// Gets/sets the available languages
    /// </summary>
    public IEnumerable<Language> Items { get; set; } = new List<Language>();

    /// <summary>
    /// Gets/sets the optional status message from the last operation.
    /// </summary>
    public StatusMessage Status { get; set; }
}
