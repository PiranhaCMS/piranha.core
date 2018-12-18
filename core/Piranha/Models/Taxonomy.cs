/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Models
{
    public class Taxonomy
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the slug.
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Operator for type casting a string to a taxonomy.
        /// </summary>
        /// <param name="str">The string</param>
        public static implicit operator Taxonomy(string str)
        {
            return new Taxonomy
            {
                Title = str
            };
        }

        /// <summary>
        /// Operator for type casting a category to a taxonomy.
        /// </summary>
        /// <param name="category">The category</param>
        public static implicit operator Taxonomy(Data.Category category)
        {
            if (category != null)
            {
                return new Taxonomy
                {
                    Id = category.Id,
                    Title = category.Title,
                    Slug = category.Slug
                };
            }
            return null;
        }

        /// <summary>
        /// Operator for type casting a tag to a taxonomy.
        /// </summary>
        /// <param name="tag">The tag</param>
        public static implicit operator Taxonomy(Data.Tag tag)
        {
            if (tag != null)
                return new Taxonomy
                {
                    Id = tag.Id,
                    Title = tag.Title,
                    Slug = tag.Slug
                };
            return null;
        }
    }
}
