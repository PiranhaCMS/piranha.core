

namespace Aero.Cms.Models;

/// <summary>
/// Simple post class for querying large sets of
/// data without loading regions or blocks.
/// </summary>
[Serializable]
public class PostInfo : PostBase, IContentInfo { }
