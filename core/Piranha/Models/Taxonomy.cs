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
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    [Serializable]
    public class Taxonomy
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the title.
        /// </summary>
        [StringLength(128)]
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the slug.
        /// </summary>
        [StringLength(128)]
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
    }
}
