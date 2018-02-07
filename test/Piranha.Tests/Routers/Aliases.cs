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
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Routers
{
    [Collection("Integration tests")]
    public class Aliases : BaseTests
    {
        private Guid SITE_ID = Guid.NewGuid();

        protected override void Init() {
            using (var api = new Api(GetDb(), storage)) {
                // Add site
                var site = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Alias Site",
                    InternalId = "AliasSite",
                    IsDefault = true
                };
                api.Sites.Save(site);

                // Add aliases
                api.Aliases.Save(new Data.Alias() {
                    Id = Guid.NewGuid(),
                    SiteId = SITE_ID,
                    AliasUrl = "/old-url",
                    RedirectUrl = "/new-url"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), storage)) {
                var aliases = api.Aliases.GetAll();
                foreach (var a in aliases)
                    api.Aliases.Delete(a);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public void GetAliasByUrl() {
            using (var api = new Api(GetDb(), storage)) {
                var response = Piranha.Web.AliasRouter.Invoke(api, "/old-url", null);

                Assert.NotNull(response);
                Assert.Equal("/new-url", response.RedirectUrl);
            }
        }

        [Fact]
        public void GetAliasByUrlNone() {
            using (var api = new Api(GetDb(), storage)) {
                var response = Piranha.Web.AliasRouter.Invoke(api, "/missing-url", null);

                Assert.Null(response);
            }
        }
    }
}
