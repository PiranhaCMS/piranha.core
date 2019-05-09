/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Linq;
using Xunit;
using Piranha.Extend;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests
{
    [Collection("Integration tests")]
    public class Config : BaseTests
    {
        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected override void Init() {
            using (var api = CreateApi()) {
                Piranha.App.Init(api);

                using (var config = new Piranha.Config(api)) {
                    config.CacheExpiresPages = 0;
                    config.CacheExpiresPosts = 0;
                    config.HierarchicalPageSlugs = true;
                    config.ManagerExpandedSitemapLevels = 0;
                }
            }
        }

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        protected override void Cleanup() { }

        [Fact]
        public void CacheExpiresPages() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.CacheExpiresPages);

                    config.CacheExpiresPages = 30;

                    Assert.Equal(30, config.CacheExpiresPages);
                }
            }
        }

        [Fact]
        public void CacheExpiresPosts() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.CacheExpiresPosts);

                    config.CacheExpiresPosts = 30;

                    Assert.Equal(30, config.CacheExpiresPosts);
                }
            }
        }

        [Fact]
        public void HierarchicalPageSlugs() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    Assert.True(config.HierarchicalPageSlugs);

                    config.HierarchicalPageSlugs = false;

                    Assert.False(config.HierarchicalPageSlugs);
                }
            }
        }

        [Fact]
        public void ManagerExpandedSitemapLevels() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.ManagerExpandedSitemapLevels);

                    config.ManagerExpandedSitemapLevels = 3;

                    Assert.Equal(3, config.ManagerExpandedSitemapLevels);
                }
            }
        }

        [Fact]
        public void MediaCDN() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    config.MediaCDN = "https://mycdn.org/uploads/";

                    Assert.Equal("https://mycdn.org/uploads/", config.MediaCDN);
                }
            }
        }

        [Fact]
        public void MediaCDNTrailingSlash() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    config.MediaCDN = "https://mycdn.org/uploads";

                    Assert.Equal("https://mycdn.org/uploads/", config.MediaCDN);
                }
            }
        }

        private IApi CreateApi()
        {
            var factory = new ContentFactory(services);
            var serviceFactory = new ContentServiceFactory(factory);

            var db = GetDb();

            return new Api(
                factory,
                new AliasRepository(db),
                new ArchiveRepository(db),
                new Piranha.Repositories.MediaRepository(db),
                new PageRepository(db, serviceFactory),
                new PageTypeRepository(db),
                new ParamRepository(db),
                new PostRepository(db, serviceFactory),
                new PostTypeRepository(db),
                new SiteRepository(db, serviceFactory),
                new SiteTypeRepository(db),
                storage: storage
            );
        }
    }
}
