/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;

namespace Piranha.Models
{
    [Serializable]
    public class TaxonomyList : List<Taxonomy>
    {
        /// <summary>
        /// Adds taxonomies from the given strings.
        /// </summary>
        /// <param name="titles">The taxonomy titles</param>
        public void Add(params string[] titles)
        {
            foreach (var title in titles)
            {
                Add(new Taxonomy
                {
                    Title = title
                });
            }
        }
    }
}