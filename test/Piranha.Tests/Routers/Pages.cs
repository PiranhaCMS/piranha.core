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
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Routers
{
    [Collection("Integration tests")]
    public class Pages : BaseTests
    {
        private Guid SITE1_ID = Guid.NewGuid();
        private Guid SITE2_ID = Guid.NewGuid();
        private Guid PAGE1_ID = Guid.NewGuid();
        private Guid PAGE2_ID = Guid.NewGuid();
        private Guid PAGE3_ID = Guid.NewGuid();

        [PageType(Title = "My PageType")]
        public class MyPage : Models.Page<MyPage>
        {
            [Region]
            public TextField Body { get; set; }
        }

        protected override void Init() {
            using (var api = CreateApi()) {
                Piranha.App.Init(api);

                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage));
                builder.Build();

                // Add site
                var site1 = new Site() {
                    Id = SITE1_ID,
                    Title = "Page Site",
                    InternalId = "PageSite",
                    IsDefault = true
                };
                api.Sites.Save(site1);

                var site2 = new Site() {
                    Id = SITE2_ID,
                    Title = "Page Site 2",
                    InternalId = "PageSite2",
                    Hostnames = "www.myothersite.com",
                    IsDefault = false
                };
                api.Sites.Save(site2);

                // Add pages
                var page1 = MyPage.Create(api);
                page1.Id = PAGE1_ID;
                page1.SiteId = SITE1_ID;
                page1.Title = "My first page";
                page1.Body = "My first body";
                page1.Published = DateTime.Now;
                api.Pages.Save(page1);

                var page2 = MyPage.Create(api);
                page2.Id = PAGE2_ID;
                page2.SiteId = SITE2_ID;
                page2.Title = "My second page";
                page2.Body = "My second body";
                page2.Published = DateTime.Now;
                api.Pages.Save(page2);

                var page3 = MyPage.Create(api);
                page3.Id = PAGE3_ID;
                page3.SiteId = SITE1_ID;
                page3.SortOrder = 1;
                page3.Title = "My third page";
                page3.Published = DateTime.Now;
                page3.RedirectUrl = "http://www.redirect.com";
                page3.RedirectType = Models.RedirectType.Temporary;
                api.Pages.Save(page3);
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var pages = api.Pages.GetAll();
                foreach (var p in pages)
                    api.Pages.Delete(p);

                var types = api.PageTypes.GetAll();
                foreach (var t in types)
                    api.PageTypes.Delete(t);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public async Task GetPageByUrlDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-first-page", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE1_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlDefaultSiteWithAction() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-first-page/action", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/page/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE1_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlDefaultSiteWithRedirect() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-third-page", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("http://www.redirect.com", response.RedirectUrl);
                Assert.Equal(Models.RedirectType.Temporary, response.RedirectType);
            }
        }

        [Fact]
        public async Task GetStartpageDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE1_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlNoneDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-second-page", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetStartpageDefaultSiteNone() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/slug", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetPageByUrlOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-second-page", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE2_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlOtherSiteWithAction() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-second-page/action", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/page/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE2_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetStartpageOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE2_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlNoneOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-first-page", SITE2_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetStartpageOtherSiteNone() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/slug", SITE2_ID);

                Assert.Null(response);
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
