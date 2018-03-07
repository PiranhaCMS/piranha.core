/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

namespace Piranha.Models
{
    public class TaxonomyList : List<Taxonomy>
    {
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