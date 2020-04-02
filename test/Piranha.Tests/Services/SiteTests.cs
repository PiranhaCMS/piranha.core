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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class SiteTestsCached : SiteTests
    {
        public override async Task InitializeAsync()
        {
            _cache = new Cache.SimpleCache();

            await base.InitializeAsync();
        }
    }

    [Collection("Integration tests")]
    public class SiteTests : BaseTestsAsync
    {
        private const string SITE_1 = "MyFirstSite";
        private const string SITE_2 = "MySecondSite";
        private const string SITE_4 = "MyFourthSite";
        private const string SITE_5 = "MyFifthSite";
        private const string SITE_6 = "MySixthSite";
        private const string SITE_1_HOSTS = "mysite.com";

        private readonly Guid SITE_1_ID = Guid.NewGuid();

        [PageType(Title = "PageType")]
        public class MyPage : Models.Page<MyPage>
        {
            [Region]
            public TextField Text { get; set; }
        }

        [SiteType(Title = "SiteType")]
        public class MySiteContent : Models.SiteContent<MySiteContent>
        {
            [Region]
            public HtmlField Header { get; set; }

            [Region]
            public HtmlField Footer { get; set; }
        }

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage));
                builder.Build();

                var siteBuilder = new SiteTypeBuilder(api)
                    .AddType(typeof(MySiteContent));
                siteBuilder.Build();

                await api.Sites.SaveAsync(new Site
                {
                    Id = SITE_1_ID,
                    SiteTypeId = "MySiteContent",
                    InternalId = SITE_1,
                    Title = SITE_1,
                    Hostnames = SITE_1_HOSTS,
                    IsDefault = true
                });

                await api.Sites.SaveAsync(new Site
                {
                    InternalId = SITE_4,
                    Title = SITE_4
                });
                await api.Sites.SaveAsync(new Site
                {
                    InternalId = SITE_5,
                    Title = SITE_5
                });
                await api.Sites.SaveAsync(new Site
                {
                    InternalId = SITE_6,
                    Title = SITE_6
                });

                // Sites for testing hostname routing
                await api.Sites.SaveAsync(new Site
                {
                    InternalId = "RoutingTest1",
                    Title = "RoutingTest1",
                    Hostnames = "mydomain.com,localhost"
                });
                await api.Sites.SaveAsync(new Site
                {
                    InternalId = "RoutingTest2",
                    Title = "RoutingTest2",
                    Hostnames = " mydomain.com/en"
                });
                await api.Sites.SaveAsync(new Site
                {
                    InternalId = "RoutingTest3",
                    Title = "RoutingTest3",
                    Hostnames = "sub.mydomain.com , sub2.localhost"
                });

                var content = await MySiteContent.CreateAsync(api);
                content.Header = "<p>Lorem ipsum</p>";
                content.Footer = "<p>Tellus Ligula</p>";
                await api.Sites.SaveContentAsync(SITE_1_ID, content);

                var page1 = await MyPage.CreateAsync(api);
                page1.SiteId = SITE_1_ID;
                page1.Title = "Startpage";
                page1.Text = "Welcome";
                page1.IsHidden = true;
                page1.Published = DateTime.Now;
                await api.Pages.SaveAsync(page1);

                var page2 = await MyPage.CreateAsync(api);
                page2.SiteId = SITE_1_ID;
                page2.SortOrder = 1;
                page2.Title = "Second page";
                page2.Text = "The second page";
                await api.Pages.SaveAsync(page2);

                var page3 = await MyPage.CreateAsync(api);
                page3.SiteId = SITE_1_ID;
                page3.ParentId = page2.Id;
                page3.Title = "Subpage";
                page3.Text = "The subpage";
                page3.Published = DateTime.Now;
                await api.Pages.SaveAsync(page3);
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllAsync(SITE_1_ID);
                foreach (var page in pages.Where(p => p.ParentId.HasValue))
                {
                    await api.Pages.DeleteAsync(page);
                }
                foreach (var page in pages.Where(p => !p.ParentId.HasValue))
                {
                    await api.Pages.DeleteAsync(page);
                }

                var types = await api.PageTypes.GetAllAsync();
                foreach (var t in types)
                {
                    await api.PageTypes.DeleteAsync(t);
                }

                var sites = await api.Sites.GetAllAsync();
                foreach (var site in sites)
                {
                    await api.Sites.DeleteAsync(site);
                }

                var siteTypes = await api.SiteTypes.GetAllAsync();
                foreach (var t in siteTypes)
                {
                    await api.SiteTypes.DeleteAsync(t);
                }
            }
        }

        [Fact]
        public void IsCached()
        {
            using (var api = CreateApi()) {
                Assert.Equal(this.GetType() == typeof(SiteTestsCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public async Task Add()
        {
            using (var api = CreateApi())
            {
                await api.Sites.SaveAsync(new Site
                {
                    InternalId = SITE_2,
                    Title = SITE_2
                });
            }
        }

        [Fact]
        public async Task AddDuplicateKey()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAnyAsync<ValidationException>(async () =>
                    await api.Sites.SaveAsync(new Site
                    {
                        InternalId = SITE_1,
                        Title = SITE_1
                    }));
            }
        }

        [Fact]
        public async Task AddEmptyFailure()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAnyAsync<ValidationException>(async () =>
                    await api.Sites.SaveAsync(new Site()));
            }
        }

        [Fact]
        public async Task AddAndGenerateInternalId()
        {
            var id = Guid.NewGuid();

            using (var api = CreateApi())
            {
                await api.Sites.SaveAsync(new Site
                {
                    Id = id,
                    Title = "Generate internal id"
                });

                var site = await api.Sites.GetByIdAsync(id);

                Assert.NotNull(site);
                Assert.Equal("GenerateInternalId", site.InternalId);
            }
        }

        [Fact]
        public async Task GetAll()
        {
            using (var api = CreateApi())
            {
                var models = await api.Sites.GetAllAsync();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public async Task GetNoneById()
        {
            using (var api = CreateApi())
            {
                var none = await api.Sites.GetByIdAsync(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneByInternalId()
        {
            using (var api = CreateApi())
            {
                var none = await api.Sites.GetByInternalIdAsync("none-existing-id");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByIdAsync(SITE_1_ID);

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public async Task GetByInternalId()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByInternalIdAsync(SITE_1);

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public async Task GetDefault()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetDefaultAsync();

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public async Task GetSitemap()
        {
            using (var api = CreateApi())
            {
                var sitemap = await api.Sites.GetSitemapAsync();

                Assert.NotNull(sitemap);
                Assert.NotEmpty(sitemap);
                Assert.Equal("Startpage", sitemap[0].Title);
            }
        }

        [Fact]
        public async Task GetSitemapById()
        {
            using (var api = CreateApi())
            {
                var sitemap = await api.Sites.GetSitemapAsync(SITE_1_ID);

                Assert.NotNull(sitemap);
                Assert.NotEmpty(sitemap);
                Assert.Equal("Startpage", sitemap[0].Title);
            }
        }

        [Fact]
        public async Task CheckPermlinkSyntax()
        {
            using (var api = CreateApi())
            {
                var sitemap = await api.Sites.GetSitemapAsync();

                foreach (var item in sitemap)
                {
                    Assert.NotNull(item.Permalink);
                    Assert.StartsWith("/", item.Permalink);
                }
            }
        }

        [Fact]
        public async Task GetUnpublishedSitemap()
        {
            using (var api = CreateApi())
            {
                var sitemap = await api.Sites.GetSitemapAsync(onlyPublished: false);

                Assert.NotNull(sitemap);
                Assert.Equal(2, sitemap.Count);
                Assert.Equal("Startpage", sitemap[0].Title);
                Assert.Single(sitemap[1].Items);
                Assert.Equal("Subpage", sitemap[1].Items[0].Title);
            }
        }

        [Fact]
        public async Task CheckHiddenSitemapItems()
        {
            using (var api = CreateApi())
            {
                var sitemap = await api.Sites.GetSitemapAsync();

                Assert.Equal(1, sitemap.Count(s => s.IsHidden));
            }
        }

        [Fact]
        public async Task ChangeDefaultSite()
        {
            using (var api = CreateApi())
            {
                var site6 = await api.Sites.GetByInternalIdAsync(SITE_6);

                Assert.False(site6.IsDefault);
                site6.IsDefault = true;
                await api.Sites.SaveAsync(site6);

                var site1 = await api.Sites.GetByIdAsync(SITE_1_ID);

                Assert.False(site1.IsDefault);
                site1.IsDefault = true;
                await api.Sites.SaveAsync(site1);
            }
        }

        [Fact]
        public async Task CantRemoveDefault()
        {
            using (var api = CreateApi())
            {
                var site1 = await api.Sites.GetByIdAsync(SITE_1_ID);

                Assert.True(site1.IsDefault);
                site1.IsDefault = false;
                await api.Sites.SaveAsync(site1);

                site1 = await api.Sites.GetByIdAsync(SITE_1_ID);

                Assert.True(site1.IsDefault);
            }
        }

        [Fact]
        public async Task GetUnpublishedSitemapById()
        {
            using (var api = CreateApi())
            {
                var sitemap = await api.Sites.GetSitemapAsync(SITE_1_ID, onlyPublished: false);

                Assert.NotNull(sitemap);
                Assert.Equal(2, sitemap.Count);
                Assert.Equal("Startpage", sitemap[0].Title);
                Assert.Single(sitemap[1].Items);
                Assert.Equal("Subpage", sitemap[1].Items[0].Title);
            }
        }

        [Fact]
        public async Task Update()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByIdAsync(SITE_1_ID);

                Assert.Equal(SITE_1_HOSTS, model.Hostnames);

                model.Hostnames = "Updated";

                await api.Sites.SaveAsync(model);
            }
        }

        [Fact]
        public async Task Delete()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByInternalIdAsync(SITE_4);

                Assert.NotNull(model);

                await api.Sites.DeleteAsync(model);
            }
        }

        [Fact]
        public async Task DeleteById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByInternalIdAsync(SITE_5);

                Assert.NotNull(model);

                await api.Sites.DeleteAsync(model.Id);
            }
        }

        [Fact]
        public async Task GetSiteContent()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetContentByIdAsync<MySiteContent>(SITE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("<p>Lorem ipsum</p>", model.Header.Value);
            }
        }

        [Fact]
        public async Task UpdateSiteContent()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetContentByIdAsync<MySiteContent>(SITE_1_ID);

                Assert.NotNull(model);
                model.Footer = "<p>Fusce Parturient</p>";
                await api.Sites.SaveContentAsync(SITE_1_ID, model);

                model = await api.Sites.GetContentByIdAsync<MySiteContent>(SITE_1_ID);
                Assert.NotNull(model);
                Assert.Equal("<p>Fusce Parturient</p>", model.Footer.Value);
            }
        }

        [Fact]
        public async Task GetDynamicSiteContent()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetContentByIdAsync(SITE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("<p>Lorem ipsum</p>", model.Regions.Header.Value);
            }
        }

        [Fact]
        public async Task UpdateDynamicSiteContent()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetContentByIdAsync(SITE_1_ID);

                Assert.NotNull(model);
                model.Regions.Footer.Value = "<p>Purus Sit</p>";
                await api.Sites.SaveContentAsync(SITE_1_ID, model);

                model = await api.Sites.GetContentByIdAsync(SITE_1_ID);
                Assert.NotNull(model);
                Assert.Equal("<p>Purus Sit</p>", model.Regions.Footer.Value);
            }
        }

        [Fact]
        public async Task GetByHostname()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByHostnameAsync("mydomain.com");

                Assert.NotNull(model);
                Assert.Equal("RoutingTest1", model.InternalId);
            }
        }

        [Fact]
        public async Task GetByHostnameSecond()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByHostnameAsync("localhost");

                Assert.NotNull(model);
                Assert.Equal("RoutingTest1", model.InternalId);
            }
        }

        [Fact]
        public async Task GetByHostnameSuffix()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByHostnameAsync("mydomain.com/en");

                Assert.NotNull(model);
                Assert.Equal("RoutingTest2", model.InternalId);
            }
        }

        [Fact]
        public async Task GetByHostnameSubdomain()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByHostnameAsync("sub.mydomain.com");

                Assert.NotNull(model);
                Assert.Equal("RoutingTest3", model.InternalId);
            }
        }

        [Fact]
        public async Task GetByHostnameSubdomainSecond()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByHostnameAsync("sub2.localhost");

                Assert.NotNull(model);
                Assert.Equal("RoutingTest3", model.InternalId);
            }
        }

        [Fact]
        public async Task GetByHostnameMissing()
        {
            using (var api = CreateApi())
            {
                var model = await api.Sites.GetByHostnameAsync("nosite.com");

                Assert.Null(model);
            }
        }
    }
}
