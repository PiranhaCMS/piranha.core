/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;
using Piranha.Runtime;

public static class PiranhaExtensions
{
    /// <summary>
    /// Adds the given taxonomies to the list.
    /// </summary>
    /// <param name="list">The list</param>
    /// <param name="titles">The taxonomies</param>
    public static void Add(this IList<Taxonomy> list, params string[] titles)
    {
        foreach (var title in titles)
        {
            list.Add(new Taxonomy
            {
                Title = title
            });
        }
    }

    /// <summary>
    /// Get a list content types by content group id
    /// </summary>
    /// <param name="list">The list</param>
    /// <param name="contentGroupId">Content group type id</param>
    public static IList<ContentType> GetByGroupId(this CachedList<ContentType> list, string contentGroupId)
    {
        return Piranha.App.ContentTypes.Where(ct => string.Equals(ct.Group, contentGroupId, StringComparison.OrdinalIgnoreCase)).ToList();
    }
}
