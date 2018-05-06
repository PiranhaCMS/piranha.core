/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Extend.Serializers;
using Piranha.Runtime;
using Xunit;

namespace Piranha.Tests
{
    public class Taxonomies
    {
        [Fact]
        public void StringToTaxonomy() {
            Models.Taxonomy t = "Test";

            Assert.NotNull(t);
            Assert.Equal(Guid.Empty, t.Id);
            Assert.Equal("Test", t.Title);
            Assert.Null(t.Slug);
        }

        [Fact]
        public void CategoryToTaxonomy() {
            var id = Guid.NewGuid();

            Models.Taxonomy t = new Data.Category {
                Id = id,
                Title = "Test",
                Slug = "test"
            };

            Assert.NotNull(t);
            Assert.Equal(id, t.Id);
            Assert.Equal("Test", t.Title);
            Assert.Equal("test", t.Slug);
        }

        [Fact]
        public void NullCategoryToTaxonomy() {
            Data.Category category = null;
            Models.Taxonomy t = category;

            Assert.Null(t);
        }

        [Fact]
        public void TagToTaxonomy() {
            var id = Guid.NewGuid();

            Models.Taxonomy t = new Data.Tag {
                Id = id,
                Title = "Test",
                Slug = "test"
            };

            Assert.NotNull(t);
            Assert.Equal(id, t.Id);
            Assert.Equal("Test", t.Title);
            Assert.Equal("test", t.Slug);
        }

        [Fact]
        public void NullTagToTaxonomy() {
            Data.Tag tag = null;
            Models.Taxonomy t = tag;

            Assert.Null(t);
        }    
    }
}