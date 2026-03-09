

namespace Aero.Cms.Security;

/// <summary>
/// The available core permissions.
/// </summary>
public static class Permission
{
    public const string PagePreview = "AeroPagePreview";
    public const string PostPreview = "AeroPostPreview";

    public static string[] All()
    {
        return new []
        {
            PagePreview,
            PostPreview
        };
    }
}
