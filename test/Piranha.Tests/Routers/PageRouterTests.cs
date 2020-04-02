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
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.Tests.Routers
{
    [Collection("Integration tests")]
    public class PageRouterTests : BaseTestsAsync
    {
        private readonly Guid SITE1_ID = Guid.NewGuid();
        private readonly Guid SITE2_ID = Guid.NewGuid();
        private readonly Guid PAGE1_ID = Guid.NewGuid();
        private readonly Guid PAGE2_ID = Guid.NewGuid();
        private readonly Guid PAGE3_ID = Guid.NewGuid();

        [PageType(Title = "My PageType")]
        public class MyPage : Models.Page<MyPage>
        {
            [Region]
            public TextField Body { get; set; }
        }

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage));
                builder.Build();

                // Add site
                var site1 = new Site
                {
                    Id = SITE1_ID,
                    Title = "Page Site",
                    InternalId = "PageSite",
                    IsDefault = true
                };
                await api.Sites.SaveAsync(site1);

                var site2 = new Site
                {
                    Id = SITE2_ID,
                    Title = "Page Site 2",
                    InternalId = "PageSite2",
                    Hostnames = "www.myothersite.com",
                    IsDefault = false
                };
                await api.Sites.SaveAsync(site2);

                // Add pages
                var page1 = await MyPage.CreateAsync(api);
                page1.Id = PAGE1_ID;
                page1.SiteId = SITE1_ID;
                page1.Title = "My first page";
                page1.Body = "My first body";
                page1.Published = DateTime.Now;
                await api.Pages.SaveAsync(page1);

                var page2 = await MyPage.CreateAsync(api);
                page2.Id = PAGE2_ID;
                page2.SiteId = SITE2_ID;
                page2.Title = "My second page";
                page2.Body = "My second body";
                page2.Published = DateTime.Now;
                await api.Pages.SaveAsync(page2);

                var page3 = await MyPage.CreateAsync(api);
                page3.Id = PAGE3_ID;
                page3.SiteId = SITE1_ID;
                page3.SortOrder = 1;
                page3.Title = "My third page";
                page3.Published = DateTime.Now;
                page3.RedirectUrl = "http://www.redirect.com";
                page3.RedirectType = Models.RedirectType.Temporary;
                await api.Pages.SaveAsync(page3);
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllAsync();
                foreach (var p in pages)
                {
                    await api.Pages.DeleteAsync(p);
                }

                var types = await api.PageTypes.GetAllAsync();
                foreach (var t in types)
                {
                    await api.PageTypes.DeleteAsync(t);
                }

                var sites = await api.Sites.GetAllAsync();
                foreach (var s in sites)
                {
                    await api.Sites.DeleteAsync(s);
                }
            }
        }

        [Fact]
        public async Task GetPageByUrlDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-first-page", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE1_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlDefaultSiteWithAction()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-first-page/action", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/page/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE1_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlDefaultSiteWithRedirect()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-third-page", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("http://www.redirect.com", response.RedirectUrl);
                Assert.Equal(Models.RedirectType.Temporary, response.RedirectType);
            }
        }

        [Fact]
        public async Task GetStartpageDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE1_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlNoneDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-second-page", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetStartpageDefaultSiteNone()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/slug", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetPageByUrlOtherSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-second-page", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE2_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlOtherSiteWithAction()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-second-page/action", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/page/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE2_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetStartpageOtherSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/page", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={PAGE2_ID}&startpage=true&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPageByUrlNoneOtherSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PageRouter.InvokeAsync(api, "/my-first-page", SITE2_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetStartpageOtherSiteNone()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.StartPageRouter.InvokeAsync(api, "/slug", SITE2_ID);

                Assert.Null(response);
            }
        }
    }
}
