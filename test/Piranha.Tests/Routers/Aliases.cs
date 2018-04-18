/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Routers
{
    [Collection("Integration tests")]
    public class Aliases : BaseTests
    {
        private Guid SITE1_ID = Guid.NewGuid();
        private Guid SITE2_ID = Guid.NewGuid();

        protected override void Init() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Add site
                var site1 = new Data.Site() {
                    Id = SITE1_ID,
                    Title = "Alias Site",
                    InternalId = "AliasSite",
                    IsDefault = true
                };
                api.Sites.Save(site1);

                var site2 = new Data.Site() {
                    Id = SITE2_ID,
                    Title = "Alias Site 2",
                    InternalId = "AliasSite2",
                    Hostnames = "www.myothersite.com",
                    IsDefault = false
                };
                api.Sites.Save(site2);

                // Add aliases
                api.Aliases.Save(new Data.Alias() {
                    Id = Guid.NewGuid(),
                    SiteId = SITE1_ID,
                    AliasUrl = "/old-url",
                    RedirectUrl = "/new-url"
                });
                api.Aliases.Save(new Data.Alias() {
                    Id = Guid.NewGuid(),
                    SiteId = SITE2_ID,
                    AliasUrl = "/old-url",
                    RedirectUrl = "/another-new-url"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var aliases = api.Aliases.GetAll();
                foreach (var a in aliases)
                    api.Aliases.Delete(a);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public void GetAliasByUrlDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.AliasRouter.Invoke(api, "/old-url", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/new-url", response.RedirectUrl);
            }
        }

        [Fact]
        public void GetAliasByUrlNoneDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.AliasRouter.Invoke(api, "/missing-url", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public void GetAliasByUrlOtherSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.AliasRouter.Invoke(api, "/old-url", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/another-new-url", response.RedirectUrl);
            }
        }

        [Fact]
        public void GetAliasByUrlNoneOtherSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.AliasRouter.Invoke(api, "/missing-url", SITE2_ID);

                Assert.Null(response);
            }
        }
    }
}
