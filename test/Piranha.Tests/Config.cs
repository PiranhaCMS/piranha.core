/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Extend;
using System.Linq;
using Xunit;

namespace Piranha.Tests
{
    [Collection("Integration tests")]
    public class Config : BaseTests
    {
        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected override void Init() {
            using (var api = new Api(GetDb(), storage)) {
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
            using (var api = new Api(GetDb(), storage)) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.CacheExpiresPages);

                    config.CacheExpiresPages = 30;

                    Assert.Equal(30, config.CacheExpiresPages);
                }
            }
        }

        [Fact]
        public void CacheExpiresPosts() {
            using (var api = new Api(GetDb(), storage)) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.CacheExpiresPosts);

                    config.CacheExpiresPosts = 30;

                    Assert.Equal(30, config.CacheExpiresPosts);
                }
            }
        }

        [Fact]
        public void HierarchicalPageSlugs() {
            using (var api = new Api(GetDb(), storage)) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(true, config.HierarchicalPageSlugs);

                    config.HierarchicalPageSlugs = false;

                    Assert.Equal(false, config.HierarchicalPageSlugs);
                }
            }
        }

        [Fact]
        public void ManagerExpandedSitemapLevels() {
            using (var api = new Api(GetDb(), storage)) {
                using (var config = new Piranha.Config(api)) {
                    Assert.Equal(0, config.ManagerExpandedSitemapLevels);

                    config.ManagerExpandedSitemapLevels = 3;

                    Assert.Equal(3, config.ManagerExpandedSitemapLevels);
                }
            }
        }
    }
}
