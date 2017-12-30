/*
 * Copyright (c) 2017 Håkan Edling
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
    public class TaxonomyList : List<Taxonomy>
    {
        public void Add(params string[] titles) {
            foreach (var title in titles) {
                Add(new Taxonomy() {
                    Title = title
                });
            }
        }
    }

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

        public static implicit operator Taxonomy(string str) {
            return new Taxonomy() {
                Title = str
            };
        }

        public static implicit operator Taxonomy(Data.Category category) {
            return new Taxonomy() {
                Id = category.Id,
                Title = category.Title,
                Slug = category.Slug
            };
        }

        public static implicit operator Taxonomy(Data.Tag tag) {
            return new Taxonomy() {
                Id = tag.Id,
                Title = tag.Title,
                Slug = tag.Slug
            };
        }   
    }
}
