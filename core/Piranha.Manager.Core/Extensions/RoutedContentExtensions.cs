using System;
using System.Collections.Generic;
using System.Text;
using Piranha.Manager.Models;
using Piranha.Models;

namespace Piranha.Manager.Extensions
{
    internal static class RoutedContentExtensions
    {
        public static string GetState(this RoutedContent post, bool isDraft)
        {
            if (post.Created == DateTime.MinValue)
            {
                return ContentState.New;
            }

            if (post.Published.HasValue)
            {
                return isDraft ? ContentState.Draft : ContentState.Published;
            }

            return ContentState.Unpublished;
        }
    }
}
