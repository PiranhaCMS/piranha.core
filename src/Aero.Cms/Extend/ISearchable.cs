

namespace Aero.Cms.Extend;

/// <summary>
/// Interface for marking a block or field as searchable.
/// </summary>
public interface ISearchable
{
    /// <summary>
    /// Gets the content that should be indexed for searching.
    /// </summary>
    string GetIndexedContent();
}
