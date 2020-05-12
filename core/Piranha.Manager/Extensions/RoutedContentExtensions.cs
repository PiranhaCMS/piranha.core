/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Piranha.Manager.Models;
using Piranha.Models;

namespace Piranha.Manager.Extensions
{
    internal static class RoutedContentExtensions
    {
        public static string GetState(this RoutedContentBase post, bool isDraft)
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
