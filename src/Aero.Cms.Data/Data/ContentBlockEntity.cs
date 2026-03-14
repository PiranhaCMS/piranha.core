

using System.Text.Json.Serialization;

namespace Aero.Cms.Data.Data;

/// <summary>
/// Basic content block.
/// </summary>
[Serializable]
public sealed class ContentBlock : BlockBase<ContentBlockField>
{
    /// <summary>
    /// Gets/sets the content id.
    /// </summary>
    public string ContentId { get; set; }

    /// <summary>
    /// Gets/sets the zero based sort index.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// Gets/sets the content.
    /// </summary>
    [JsonIgnore]
    public Content Content { get; set; }
}
