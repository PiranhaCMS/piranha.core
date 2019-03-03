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
using System.Data.SqlClient;
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
    public class Posts : BaseTests
    {
        private Guid SITE1_ID = Guid.NewGuid();
        private Guid SITE2_ID = Guid.NewGuid();
        private Guid PAGE1_ID = Guid.NewGuid();
        private Guid CATEGORY1_ID = Guid.NewGuid();
        private Guid POST1_ID = Guid.NewGuid();
        private Guid PAGE2_ID = Guid.NewGuid();
        private Guid CATEGORY2_ID = Guid.NewGuid();
        private Guid POST2_ID = Guid.NewGuid();
        private Guid TAG1_ID = Guid.NewGuid();
        private Guid TAG2_ID = Guid.NewGuid();

        [PageType(Title = "My PageType")]
        public class MyPage : Models.ArchivePage<MyPage>
        {
        }

        [PostType(Title = "My PostType")]
        public class MyPost : Models.Post<MyPost>
        {
            [Region]
            public TextField Body { get; set; }
        }

        protected override void Init() {
            using (var api = CreateApi()) {
                Piranha.App.Init(api);

                var pageBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage));
                pageBuilder.Build();

                var postBuilder = new PostTypeBuilder(api)
                    .AddType(typeof(MyPost));
                postBuilder.Build();

                // Add site
                var site1 = new Site() {
                    Id = SITE1_ID,
                    Title = "Page Site",
                    InternalId = "PostSite",
                    IsDefault = true
                };
                api.Sites.Save(site1);

                var site2 = new Site() {
                    Id = SITE2_ID,
                    Title = "Page Site 2",
                    InternalId = "PostSite2",
                    Hostnames = "www.myothersite.com",
                    IsDefault = false
                };
                api.Sites.Save(site2);

                // Add pages
                var page1 = MyPage.Create(api);
                page1.Id = PAGE1_ID;
                page1.SiteId = SITE1_ID;
                page1.Title = "Blog";
                page1.Published = DateTime.Now;
                api.Pages.Save(page1);

                var page2 = MyPage.Create(api);
                page2.Id = PAGE2_ID;
                page2.SiteId = SITE2_ID;
                page2.Title = "News";
                page2.Published = DateTime.Now;
                api.Pages.Save(page2);

                // Add categories
                var category1 = new Models.Taxonomy() {
                    Id = CATEGORY1_ID,
                    Title = "Default category"
                };

                var category2 = new Models.Taxonomy() {
                    Id = CATEGORY2_ID,
                    Title = "Default category"
                };

                // Add posts
                var post1 = MyPost.Create(api);
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
                api.Posts.Save(post1);

                var post2 = MyPost.Create(api);
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
                api.Posts.Save(post2);
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var pages = api.Pages.GetAll();
                foreach (var p in pages)
                    api.Pages.Delete(p);

                var pageTypes = api.PageTypes.GetAll();
                foreach (var t in pageTypes)
                    api.PageTypes.Delete(t);

                var postTypes = api.PostTypes.GetAll();
                foreach (var t in postTypes)
                    api.PostTypes.Delete(t);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public async Task GetPostByUrlDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/blog/my-first-post", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/post", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST1_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlDefaultSiteWithAction() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/blog/my-first-post/action", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/post/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST1_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlNoneDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/news/my-second-page", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetArchiveDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/2018", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearMonthDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/2018/2", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearMonthPageDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/2018/2/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=1&pagenum=1&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveYearMonthPageCategoryDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/category/default-category/2018/2/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=1&pagenum=1&category={CATEGORY1_ID}&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveCategoryDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/category/default-category", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category={CATEGORY1_ID}&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveMissingCategoryDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/category/missing-category", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category={Guid.Empty}&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveTagDefaultSite() {
            using (var api = CreateApi()) {
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
        public async Task GetArchivePageDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=1&pagenum=1&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveNoneDefaultSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/news", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetPostByUrlOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/news/my-second-post", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/post", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST2_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlOtherSiteAction() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/news/my-second-post/action", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/post/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST2_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetPostByUrlNoneOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.PostRouter.InvokeAsync(api, "/blog/my-first-post", SITE2_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public async Task GetArchiveOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/news", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE2_ID}&year=&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public async Task GetArchiveNoneOtherSite() {
            using (var api = CreateApi()) {
                var response = await Piranha.Web.ArchiveRouter.InvokeAsync(api, "/blog", SITE2_ID);

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
