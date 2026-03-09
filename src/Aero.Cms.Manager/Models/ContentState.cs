

namespace Aero.Cms.Manager.Models;

/// <summary>
/// The different states revision based content can have.
/// </summary>
public static class ContentState
{
    public static string New { get; } = "new";
    public static string Unpublished { get; } = "unpublished";
    public static string Published { get; } = "published";
    public static string Draft { get; } = "draft";
}
