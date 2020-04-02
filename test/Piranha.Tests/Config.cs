/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Threading.Tasks;
using Xunit;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests
{
    [Collection("Integration tests")]
    public class Config : BaseTestsAsync
    {
        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        public override Task InitializeAsync()
        {
            return Task.Run(() =>
            {
                using (var api = CreateApi())
                {

                    using (var config = new Piranha.Config(api)) {
                        config.ArchivePageSize = 0;
                        config.CacheExpiresPages = 0;
                        config.CacheExpiresPosts = 0;
                        config.CommentsApprove = true;
                        config.CommentsPageSize = 0;
                        config.HierarchicalPageSlugs = true;
                        config.ManagerExpandedSitemapLevels = 0;
                    }
                }
            });
        }

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        public override Task DisposeAsync()
        {
            return Task.Run(() => {});
        }

        [Fact]
        public void ArchivePageSize() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.ArchivePageSize);

                    config.ArchivePageSize = 5;

                    Assert.Equal(5, config.ArchivePageSize);
                }
            }
        }

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
        public void CommentsApprove() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    Assert.True(config.CommentsApprove);

                    config.CommentsApprove = false;

                    Assert.False(config.CommentsApprove);
                }
            }
        }

        [Fact]
        public void CommentsPageSize() {
            using (var api = CreateApi()) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.CommentsPageSize);

                    config.CommentsPageSize = 5;

                    Assert.Equal(5, config.CommentsPageSize);
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
    }
}
