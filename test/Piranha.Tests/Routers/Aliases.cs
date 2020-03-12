/*
 * Copyright (c) 2018-2019 Håkan Edling
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
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Routers
{
    [Collection("Integration tests")]
    public class Aliases : BaseTests
    {
        private readonly Guid SITE1_ID = Guid.NewGuid();
        private readonly Guid SITE2_ID = Guid.NewGuid();

        protected override void Init() {
            using (var api = CreateApi()) {
                // Add site
                var site1 = new Site
                {
                    Id = SITE1_ID,
                    Title = "Alias Site",
                    InternalId = "AliasSite",
                    IsDefault = true
                };
                api.Sites.Save(site1);

                var site2 = new Site
                {
                    Id = SITE2_ID,
                    Title = "Alias Site 2",
                    InternalId = "AliasSite2",
                    Hostnames = "www.myothersite.com",
                    IsDefault = false
                };
                api.Sites.Save(site2);

                // Add aliases
                api.Aliases.Save(new Alias
                {
                    Id = Guid.NewGuid(),
                    SiteId = SITE1_ID,
                    AliasUrl = "/old-url",
                    RedirectUrl = "/new-url"
                });
                api.Aliases.Save(new Alias
                {
                    Id = Guid.NewGuid(),
                    SiteId = SITE2_ID,
                    AliasUrl = "/old-url",
                    RedirectUrl = "/another-new-url"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var aliases = api.Aliases.GetAll();
                foreach (var a in aliases)
                    api.Aliases.Delete(a);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
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

        private IApi CreateApi()
        {
            var factory = new LegacyContentFactory(services);
            var serviceFactory = new ContentServiceFactory(factory);

            var db = GetDb();

            return new Api(
                factory,
                new AliasRepository(db),
                new ArchiveRepository(db),
                new ContentTypeRepository(db),
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
