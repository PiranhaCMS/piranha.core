

namespace Aero.Cms.Extend;

/// <summary>
/// Service for converting markdown to HTML.
/// </summary>
public interface IMarkdown
{
    /// <summary>
    /// Transforms the given markdown string to html.
    /// </summary>
    /// <param name="md">The markdown</param>
    /// <returns>The transformed html</returns>
    string Transform(string md);
}
