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
    public class PostRouterTests : BaseTestsAsync
    {
        private readonly Guid SITE1_ID = Guid.NewGuid();
        private readonly Guid SITE2_ID = Guid.NewGuid();
        private readonly Guid PAGE1_ID = Guid.NewGuid();
        private readonly Guid CATEGORY1_ID = Guid.NewGuid();
        private readonly Guid POST1_ID = Guid.NewGuid();
        private readonly Guid PAGE2_ID = Guid.NewGuid();
        private readonly Guid CATEGORY2_ID = Guid.NewGuid();
        private readonly Guid POST2_ID = Guid.NewGuid();
        private readonly Guid TAG1_ID = Guid.NewGuid();
        private readonly Guid TAG2_ID = Guid.NewGuid();

        [PageType(Title = "My PageType", IsArchive = true)]
        public class MyPage : Models.Page<MyPage>
        {
        }

        [PostType(Title = "My PostType")]
        public class MyPost : Models.Post<MyPost>
        {
            [Region]
            public TextField Body { get; set; }
        }

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                var pageBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage));
                pageBuilder.Build();

                var postBuilder = new PostTypeBuilder(api)
                    .AddType(typeof(MyPost));
                postBuilder.Build();

                // Add site
                var site1 = new Site
                {
                    Id = SITE1_ID,
                    Title = "Page Site",
                    InternalId = "PostSite",
                    IsDefault = true
                };
                await api.Sites.SaveAsync(site1);

                var site2 = new Site
                {
                    Id = SITE2_ID,
                    Title = "Page Site 2",
                    InternalId = "PostSite2",
                    Hostnames = "www.myothersite.com",
                    IsDefault = false
                };
                await api.Sites.SaveAsync(site2);

                // Add pages
                var page1 = await MyPage.CreateAsync(api);
                page1.Id = PAGE1_ID;
                page1.SiteId = SITE1_ID;
                page1.Title = "Blog";
                page1.Published = DateTime.Now;
                await api.Pages.SaveAsync(page1);

                var page2 = await MyPage.CreateAsync(api);
                page2.Id = PAGE2_ID;
                page2.SiteId = SITE2_ID;
                page2.Title = "News";
                page2.Published = DateTime.Now;
                await api.Pages.SaveAsync(page2);

                // Add categories
                var category1 = new Models.Taxonomy
                {
                    Id = CATEGORY1_ID,
                    Title = "Default category"
                };

                var category2 = new Models.Taxonomy
                {
                    Id = CATEGORY2_ID,
                    Title = "Default category"
                };

                // Add posts
                var post1 = await MyPost.CreateAsync(api);
                post1.Id = POST1_ID;
                post1.BlogId = page1.Id;
                post1.Category = category1;
                post1.Title = "My first post";
                post1.Body = "My first body";
                post1.Tags.Add(new Models.Taxonomy
                {
                    Id = TAG1_ID,
                    Title = "My tag"
                });
                post1.Published = DateTime.Now;
                await api.Posts.SaveAsync(post1);

                var post2 = await MyPost.CreateAsync(api);
                post2.Id = POST2_ID;
                post2.BlogId = page2.Id;
                post2.Category = category2;
                post2.Title = "My second post";
                post2.Body = "My second body";
                post2.Tags.Add(new Models.Taxonomy
                {
                    Id = TAG2_ID,
                    Title = "My other tag"
                });
                post2.Published = DateTime.Now;
                await api.Posts.SaveAsync(post2);
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

                var pageTypes = await api.PageTypes.GetAllAsync();
                foreach (var t in pageTypes)
                {
                    await api.PageTypes.DeleteAsync(t);
                }

                var postTypes = await api.PostTypes.GetAllAsync();
                foreach (var t in postTypes)
                {
                    await api.PostTypes.DeleteAsync(t);
                }

                var sites = await api.Sites.GetAllAsync();
                foreach (var s in sites)
                {
                    await api.Sites.DeleteAsync(s);
                }
            }
        }

        [Fact]
        public async Task GetPostByUrlDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/blog/my-first-post", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/post", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST1_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlDefaultSiteWithAction()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/blog/my-first-post/action", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/post/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST1_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlNoneDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/news/my-second-page", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetArchiveDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/2018", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearMonthDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/2018/2", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearMonthPageDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/2018/2/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=1&pagenum=1&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearMonthPageCategoryDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/category/default-category/2018/2/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=1&pagenum=1&category={CATEGORY1_ID}&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveCategoryDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/category/default-category", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category={CATEGORY1_ID}&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveMissingCategoryDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/category/missing-category", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category={Guid.Empty}&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveTagDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/tag/my-tag", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category=&tag={TAG1_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveMissingTagDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/tag/my-other-tag", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category=&tag={Guid.Empty}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchivePageDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=1&pagenum=1&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveNoneDefaultSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/news", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetPostByUrlOtherSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/news/my-second-post", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/post", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST2_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlOtherSiteAction()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/news/my-second-post/action", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/post/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST2_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlNoneOtherSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/blog/my-first-post", SITE2_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetArchiveOtherSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/news", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE2_ID}&year=&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveNoneOtherSite()
        {
            using (var api = CreateApi())
            {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog", SITE2_ID);

                Assert.Null(response);
            }
        }
    }
}
