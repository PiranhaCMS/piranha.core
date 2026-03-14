

using Aero.Cms.Manager.Models;
using Aero.Cms.Models;

namespace Aero.Cms.Manager.Extensions;

internal static class RoutedContentExtensions
{
    public static string GetState(this RoutedContentBase content, bool isDraft)
    {
        if (content.Created == DateTime.MinValue)
        {
            return ContentState.New;
        }

        if (content.Published.HasValue)
        {
            return isDraft ? ContentState.Draft : ContentState.Published;
        }

        return ContentState.Unpublished;
    }
}
