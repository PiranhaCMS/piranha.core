/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Models;
using System;
using Xunit;

namespace Piranha.Tests
{
    public class Sitemaps : BaseTests
    {
        private Sitemap sitemap;
        private string id_1 = Guid.NewGuid().ToString();
        private string id_2 = Guid.NewGuid().ToString();

        protected override void Init() {
            sitemap = new Sitemap();

            sitemap.Add(new SitemapItem() {
                Id = id_1,
            });
            sitemap[0].Items.Add(new SitemapItem() {
                Id = id_2
            });
            sitemap[0].Items.Add(new SitemapItem() {
                Id = Guid.NewGuid().ToString()
            });
        }

        protected override void Cleanup() {}

        [Fact]
        public void GetPartial() {
            var partial = sitemap.GetPartial(id_1);

            Assert.NotNull(partial);
            Assert.Equal(2, partial.Count);
            Assert.Equal(id_2, partial[0].Id);
        }

        [Fact]
        public void GetPartialMissing() {
            var partial = sitemap.GetPartial(Guid.NewGuid().ToString());

            Assert.Null(partial);
        }

        [Fact]
        public void HasChild() {
            Assert.True(sitemap[0].HasChild(id_2));
        }

        [Fact]
        public void HasChildMissing() {
            Assert.False(sitemap[0].HasChild(Guid.NewGuid().ToString()));
        }

        [Fact]
        public void GetBreadcrumb() {
            var crumb = sitemap.GetBreadcrumb(id_2);

            Assert.NotNull(crumb);
            Assert.Equal(2, crumb.Count);
            Assert.Equal(id_1, crumb[0].Id);
            Assert.Equal(id_2, crumb[1].Id);
        }

        [Fact]
        public void GetBreadcrumbMissing() {
            var crumb = sitemap.GetBreadcrumb(Guid.NewGuid().ToString());

            Assert.Null(crumb);
        }
    }
}