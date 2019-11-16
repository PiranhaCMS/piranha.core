/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using Piranha.Models;

public static class PiranhaModelExtensions
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
}
