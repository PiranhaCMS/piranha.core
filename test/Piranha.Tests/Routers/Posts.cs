/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

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
        public class MyPage : Models.BlogPage<MyPage>
        {
        }

        [PostType(Title = "My PostType")]
        public class MyPost : Models.Post<MyPost>
        {
            [Region]
            public TextField Body { get; set; }
        }
        

        protected override void Init() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Piranha.App.Init();

                var pageBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage));
                pageBuilder.Build();

                var postBuilder = new PostTypeBuilder(api)
                    .AddType(typeof(MyPost));
                postBuilder.Build();

                // Add site
                var site1 = new Data.Site() {
                    Id = SITE1_ID,
                    Title = "Page Site",
                    InternalId = "PostSite",
                    IsDefault = true
                };
                api.Sites.Save(site1);

                var site2 = new Data.Site() {
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
                var category1 = new Data.Category() {
                    Id = CATEGORY1_ID,
                    BlogId = PAGE1_ID,
                    Title = "Default category"
                };
                api.Categories.Save(category1);
                
                var category2 = new Data.Category() {
                    Id = CATEGORY2_ID,
                    BlogId = PAGE2_ID,
                    Title = "Default category"
                };
                api.Categories.Save(category2);

                // Add tags
                var tag = new Data.Tag() {
                    Id = TAG1_ID,
                    BlogId = PAGE1_ID,
                    Title = "My tag"
                };
                api.Tags.Save(tag);

                tag = new Data.Tag() {
                    Id = TAG2_ID,
                    BlogId = PAGE2_ID,
                    Title = "My other tag"
                };
                api.Tags.Save(tag);

                // Add posts
                var post1 = MyPost.Create(api);
                post1.Id = POST1_ID;
                post1.BlogId = page1.Id;
                post1.Category = category1;
                post1.Title = "My first post";
                post1.Body = "My first body";
                post1.Tags.Add("My tag");
                post1.Published = DateTime.Now;
                api.Posts.Save(post1);

                var post2 = MyPost.Create(api);
                post2.Id = POST2_ID;
                post2.BlogId = page2.Id;
                post2.Category = category2;
                post2.Title = "My second post";
                post2.Body = "My second body";
                post2.Tags.Add("My other tag");
                post2.Published = DateTime.Now;
                api.Posts.Save(post2);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
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
        public void GetPostByUrlDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.PostRouter.Invoke(api, "/blog/my-first-post", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/post", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST1_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public void GetPostByUrlDefaultSiteWithAction() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.PostRouter.Invoke(api, "/blog/my-first-post/action", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/post/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST1_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public void GetPostByUrlNoneDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.PostRouter.Invoke(api, "/news/my-second-page", SITE1_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public void GetArchiveDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveYearDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/2018", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveYearMonthDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/2018/2", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveYearMonthPageDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/2018/2/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=1&pagenum=1&category=&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveYearMonthPageCategoryDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/category/default-category/2018/2/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=2018&month=2&page=1&pagenum=1&category={CATEGORY1_ID}&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveCategoryDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/category/default-category", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category={CATEGORY1_ID}&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveMissingCategoryDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/category/missing-category", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category={Guid.Empty}&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveTagDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/tag/my-tag", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category=&tag={TAG1_ID}&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveMissingTagDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/tag/my-other-tag", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=&pagenum=&category=&tag={Guid.Empty}&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchivePageDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog/page/1", SITE1_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE1_ID}&year=&month=&page=1&pagenum=1&category=&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveNoneDefaultSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/news", SITE1_ID);

                Assert.Null(response);
            }            
        }

        [Fact]
        public void GetPostByUrlOtherSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.PostRouter.Invoke(api, "/news/my-second-post", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/post", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST2_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public void GetPostByUrlOtherSiteAction() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.PostRouter.Invoke(api, "/news/my-second-post/action", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/post/action", response.Route);
                Assert.True(response.IsPublished);
                Assert.Equal($"id={POST2_ID}&piranha_handled=true", response.QueryString);
            }
        }

        [Fact]
        public void GetPostByUrlNoneOtherSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.PostRouter.Invoke(api, "/blog/my-first-post", SITE2_ID);

                Assert.Null(response);
            }
        }

        [Fact]
        public void GetArchiveOtherSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/news", SITE2_ID);

                Assert.NotNull(response);
                Assert.Equal("/archive", response.Route);
                Assert.Equal($"id={PAGE2_ID}&year=&month=&page=&pagenum=&category=&tag=&piranha_handled=true", response.QueryString);
            }            
        }

        [Fact]
        public void GetArchiveNoneOtherSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                var response = Piranha.Web.ArchiveRouter.Invoke(api, "/blog", SITE2_ID);

                Assert.Null(response);
            }            
        }
    }
}
