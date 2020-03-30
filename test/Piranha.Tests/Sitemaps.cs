/*
 * Copyright (c) .NET Foundation and Contributors
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
        private readonly Guid id_1 = Guid.NewGuid();
        private readonly Guid id_2 = Guid.NewGuid();

        protected override void Init() {
            sitemap = new Sitemap();

            sitemap.Add(new SitemapItem
            {
                Id = id_1,
                Title = "No navigation title"
            });
            sitemap[0].Items.Add(new SitemapItem
            {
                Id = id_2,
                Title = "Has navigation title",
                NavigationTitle = "Navigation title"
            });
            sitemap[0].Items.Add(new SitemapItem
            {
                Id = Guid.NewGuid()
            });
        }

        protected override void Cleanup()
        {
            //No need to clean up
        }

        [Fact]
        public void GetNoNavigationTitle() {
            var item = sitemap[0];

            Assert.Equal("No navigation title", item.MenuTitle);
        }

        [Fact]
        public void GetNavigationTitle() {
            var item = sitemap[0].Items[0];

            Assert.Equal("Navigation title", item.MenuTitle);
        }

        [Fact]
        public void GetPartial() {
            var partial = sitemap.GetPartial(id_1);

            Assert.NotNull(partial);
            Assert.Equal(2, partial.Count);
            Assert.Equal(id_2, partial[0].Id);
        }

        [Fact]
        public void GetPartialMissing() {
            var partial = sitemap.GetPartial(Guid.NewGuid());

            Assert.Null(partial);
        }

        [Fact]
        public void HasChild() {
            Assert.True(sitemap[0].HasChild(id_2));
        }

        [Fact]
        public void HasChildMissing() {
            Assert.False(sitemap[0].HasChild(Guid.NewGuid()));
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
            var crumb = sitemap.GetBreadcrumb(Guid.NewGuid());

            Assert.Null(crumb);
        }
    }
}