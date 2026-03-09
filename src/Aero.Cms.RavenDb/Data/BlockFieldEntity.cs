

using System.Text.Json.Serialization;

namespace Aero.Cms.RavenDb.Data;

/// <summary>
/// Content field for a block.
/// </summary>
[Serializable]
public sealed class BlockField : BlockFieldBase
{
    /// <summary>
    /// Gets/sets the block containing the field.
    /// </summary>
    [JsonIgnore]
    public Block Block { get; set; }
}
