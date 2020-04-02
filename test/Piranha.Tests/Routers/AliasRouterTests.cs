/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Threading.Tasks;
using Xunit;
using Piranha.Models;

namespace Piranha.Tests.Routers
{
    [Collection("Integration tests")]
    public class AliasRouterTests : BaseTestsAsync
    {
        private readonly Guid SITE1_ID = Guid.NewGuid();
        private readonly Guid SITE2_ID = Guid.NewGuid();

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                // Add site
                var site1 = new Site
                {
                    Id = SITE1_ID,
                    Title = "Alias Site",
                    InternalId = "AliasSite",
                    IsDefault = true
                };
                await api.Sites.SaveAsync(site1);

                var site2 = new Site
                {
                    Id = SITE2_ID,
                    Title = "Alias Site 2",
                    InternalId = "AliasSite2",
                    Hostnames = "www.myothersite.com",
                    IsDefault = false
                };
                await api.Sites.SaveAsync(site2);

                // Add aliases
                await api.Aliases.SaveAsync(new Alias
                {
                    Id = Guid.NewGuid(),
                    SiteId = SITE1_ID,
                    AliasUrl = "/old-url",
                    RedirectUrl = "/new-url"
                });
                await api.Aliases.SaveAsync(new Alias
                {
                    Id = Guid.NewGuid(),
                    SiteId = SITE2_ID,
                    AliasUrl = "/old-url",
                    RedirectUrl = "/another-new-url"
                });
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var aliases = await api.Aliases.GetAllAsync();
                foreach (var a in aliases)
                {
                    await api.Aliases.DeleteAsync(a);
                }

                var sites = await api.Sites.GetAllAsync();
                foreach (var s in sites)
                {
                    await api.Sites.DeleteAsync(s);
                }
            }
        }

        [Fact]
        public async Task GetAliasByUrlDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.AliasRouter.InvokeAsync(api, "/old-url", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/new-url", response.RedirectUrl);
            }
        }

        [Fact]
        public async Task GetAliasByUrlNoneDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.AliasRouter.InvokeAsync(api, "/missing-url", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetAliasByUrlOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.AliasRouter.InvokeAsync(api, "/old-url", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/another-new-url", response.RedirectUrl);
            }
        }

        [Fact]
        public async Task GetAliasByUrlNoneOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.AliasRouter.InvokeAsync(api, "/missing-url", SITE2_ID);

                Assert.Null(response);
            }
        }
    }
}
