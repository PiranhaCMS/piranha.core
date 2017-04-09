/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class Pages : BaseTests
    {
        #region Members
        public static readonly string SITE_ID = new Guid("8170A7B1-00C5-4014-90BD-6401710E93BD").ToString();
        public static readonly string PAGE_1_ID = new Guid("C86E47BA-8D0A-4A54-8EB2-2EE2E05E79D5").ToString();
        public static readonly string PAGE_2_ID = new Guid("BF61DBD2-BA79-4AF5-A4F5-66331AC3213E").ToString();
        public static readonly string PAGE_3_ID = new Guid("AF62769D-019F-44BF-B44F-E3D61F421DD2").ToString();
        #endregion

        [PageType(Title = "My PageType")]
        public class MyPage : Models.Page<MyPage>
        {
            [Region]
            public TextField Ingress { get; set; }
            [Region]
            public MarkdownField Body { get; set; }
        }

        [PageType(Title = "My CollectionPage")]
        public class MyCollectionPage : Models.Page<MyCollectionPage>
        {
            [Region]
            public IList<TextField> Texts { get; set; }

            public MyCollectionPage() {
                Texts = new List<TextField>();
            }
        }

        protected override void Init() {
            using (var api = new Api(options, storage)) {
                Piranha.App.Init(api);

                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage))
                    .AddType(typeof(MyCollectionPage));
                builder.Build();

                var site = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Default Site",
                    InternalId = "DefaultSite",
                    IsDefault = true
                };
                api.Sites.Save(site);

                var page1 = MyPage.Create(api);
                page1.Id = PAGE_1_ID;
                page1.SiteId = SITE_ID;
                page1.Title = "My first page";
                page1.Ingress = "My first ingress";
                page1.Body = "My first body";
                api.Pages.Save(page1);

                var page2 = MyPage.Create(api);
                page2.Id = PAGE_2_ID;
                page2.SiteId = SITE_ID;
                page2.Title = "My second page";
                page2.Ingress = "My second ingress";
                page2.Body = "My second body";
                api.Pages.Save(page2);

                var page3 = MyPage.Create(api);
                page3.Id = PAGE_3_ID;
                page3.SiteId = SITE_ID;
                page3.Title = "My third page";
                page3.Ingress = "My third ingress";
                page3.Body = "My third body";
                api.Pages.Save(page3);

                var page4 = MyCollectionPage.Create(api);
                page4.SiteId = SITE_ID;
                page4.Title = "My collection page";
                page4.SortOrder = 1;
                page4.Texts.Add(new TextField() {
                    Value = "First text"
                });
                page4.Texts.Add(new TextField() {
                    Value = "Second text"
                });
                page4.Texts.Add(new TextField() {
                    Value = "Third text"
                });
                api.Pages.Save(page4);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(options, storage)) {
                var pages = api.Pages.GetAll(SITE_ID);
                foreach (var page in pages)
                    api.Pages.Delete(page);

                var types = api.PageTypes.GetAll();
                foreach (var t in types)
                    api.PageTypes.Delete(t);

                var site = api.Sites.GetById(SITE_ID);
                if (site != null)
                    api.Sites.Delete(site);
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(options, storage)) {
                var none = api.Pages.GetById(Guid.NewGuid().ToString());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(options, storage)) {
                var none = api.Pages.GetBySlug("none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetStartpage() {
            using (var api = new Api(options, storage)) {
                var model = api.Pages.GetStartpage();

                Assert.NotNull(model);
                Assert.Equal(null, model.ParentId);
                Assert.Equal(0, model.SortOrder);
            }
        }

        [Fact]
        public void GetStartpageBySite() {
            using (var api = new Api(options, storage)) {
                var model = api.Pages.GetStartpage(SITE_ID);

                Assert.NotNull(model);
                Assert.Equal(null, model.ParentId);
                Assert.Equal(0, model.SortOrder);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(options, storage)) {
                var pages = api.Pages.GetAll(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }
        }

        [Fact]
        public void GetGenericById() {
            using (var api = new Api(options, storage)) {
                var model = api.Pages.GetById<MyPage>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetGenericBySlug() {
            using (var api = new Api(options, storage)) {
                var model = api.Pages.GetBySlug<MyPage>("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicById() {
            using (var api = new Api(options, storage)) {
                var model = api.Pages.GetById(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicBySlug() {
            using (var api = new Api(options, storage)) {
                var model = api.Pages.GetBySlug("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("My first page", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetCollectionPage() {
            using (var api = new Api(options, storage)) {
                var page = api.Pages.GetBySlug<MyCollectionPage>("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(3, page.Texts.Count);
                Assert.Equal("Second text", page.Texts[1].Value);
            }
        }

        [Fact]
        public void GetDynamicCollectionPage() {
            using (var api = new Api(options, storage)) {
                var page = api.Pages.GetBySlug("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(3, page.Regions.Texts.Count);
                Assert.Equal("Second text", page.Regions.Texts[1].Value);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(options, storage)) {
                var page = MyPage.Create(api, "MyPage");
                page.SiteId = SITE_ID;
                page.Title = "My fourth page";
                page.Ingress = "My fourth ingress";
                page.Body = "My fourth body";

                api.Pages.Save(page);

                Assert.Equal(5, api.Pages.GetAll(SITE_ID).Count());
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(options, storage)) {
                var page = api.Pages.GetById<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal("My first page", page.Title);

                page.Title = "Updated page";
                page.IsHidden = true;
                api.Pages.Save(page);

                page = api.Pages.GetById<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal("Updated page", page.Title);
                Assert.Equal(true, page.IsHidden);
            }
        }

        [Fact]
        public void Move() {
            using (var api = new Api(options, storage)) {
                var page = api.Pages.GetById(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.True(page.SortOrder > 0);

                page.SortOrder = 0;
                api.Pages.Move(page, null, 0);

                page = api.Pages.GetById(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal(0, page.SortOrder);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(options, storage)) {
                var page = api.Pages.GetById<MyPage>(PAGE_3_ID);
                var count = api.Pages.GetAll(SITE_ID).Count();

                Assert.NotNull(page);

                api.Pages.Delete(page);

                Assert.Equal(count - 1, api.Pages.GetAll(SITE_ID).Count());
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(options, storage)) {
                var count = api.Pages.GetAll(SITE_ID).Count();

                api.Pages.Delete(PAGE_2_ID);

                Assert.Equal(count - 1, api.Pages.GetAll(SITE_ID).Count());
            }
        }
    }
}
