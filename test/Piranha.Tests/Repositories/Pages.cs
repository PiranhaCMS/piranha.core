/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.Extensions.DependencyInjection;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class PagesCached : Pages
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Pages : BaseTests
    {
        #region Members
        public static readonly Guid SITE_ID = Guid.NewGuid();
        public static readonly Guid SITE_EMPTY_ID = Guid.NewGuid();
        public static readonly Guid PAGE_1_ID = Guid.NewGuid();
        public static readonly Guid PAGE_2_ID = Guid.NewGuid();
        public static readonly Guid PAGE_3_ID = Guid.NewGuid();
        public static readonly Guid PAGE_7_ID = Guid.NewGuid();
        public static readonly Guid PAGE_8_ID = Guid.NewGuid();
        public static readonly Guid PAGE_DI_ID = Guid.NewGuid();
        protected ICache cache;
        #endregion

        public interface IMyService {
            string Value { get; }
        }

        public class MyService : IMyService {
            public string Value { get; private set; }

            public MyService() {
                Value = "My service value";
            }
        }        

        [Piranha.Extend.FieldType(Name = "Fourth")]
        public class MyFourthField : Extend.Fields.SimpleField<string> {
            public void Init(IMyService myService) {
                Value = myService.Value;
            }
        }        

        public class ComplexRegion
        {
            [Field]
            public StringField Title { get; set; }
            [Field]
            public TextField Body { get; set; }
        }

        [PageType(Title = "My PageType")]
        public class MyPage : Models.Page<MyPage>
        {
            [Region]
            public TextField Ingress { get; set; }
            [Region]
            public MarkdownField Body { get; set; }
        }

        [PageType(Title = "My BlogType")]
        public class MyBlogPage : Models.ArchivePage<MyBlogPage>
        {
            [Region]
            public TextField Ingress { get; set; }
            [Region]
            public MarkdownField Body { get; set; }
        }

        [PageType(Title = "Missing PageType")]
        public class MissingPage : Models.Page<MissingPage>
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
            [Region]
            public IList<ComplexRegion> Teasers { get; set; }

            public MyCollectionPage() {
                Texts = new List<TextField>();
                Teasers = new List<ComplexRegion>();
            }
        }

        [PageType(Title = "Injection PageType")]
        public class MyDIPage : Models.Page<MyDIPage>
        {
            [Region]
            public MyFourthField Body { get; set; }
        }
        

        protected override void Init() {
            services = new ServiceCollection()
                .AddSingleton<IMyService, MyService>()
                .BuildServiceProvider();

            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Piranha.App.Init();

                Piranha.App.Fields.Register<MyFourthField>();

                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(MissingPage))
                    .AddType(typeof(MyBlogPage))
                    .AddType(typeof(MyPage))
                    .AddType(typeof(MyCollectionPage))
                    .AddType(typeof(MyDIPage));
                builder.Build();
                
                var site = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Default Site",
                    InternalId = "DefaultSite",
                    IsDefault = true
                };
                var emptysite = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Empty Site",
                    InternalId = "EmptySite",
                    IsDefault = false
                };
                api.Sites.Save(site);
                api.Sites.Save(emptysite);

                var page1 = MyPage.Create(api);
                page1.Id = PAGE_1_ID;
                page1.SiteId = SITE_ID;
                page1.Title = "My first page";
                page1.Ingress = "My first ingress";
                page1.Body = "My first body";
                page1.Blocks.Add(new Extend.Blocks.TextBlock {
                    Body = "Sollicitudin Aenean"
                });
                page1.Blocks.Add(new Extend.Blocks.TextBlock {
                    Body = "Ipsum Elit"
                });
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

                var page5 = MyBlogPage.Create(api);
                page5.SiteId = SITE_ID;
                page5.Title = "Blog Archive";
                api.Pages.Save(page5);

                var page6 = MyDIPage.Create(api);
                page6.Id = PAGE_DI_ID;
                page6.SiteId = SITE_ID;
                page6.Title = "My Injection Page";
                api.Pages.Save(page6);

                var page7 = MyPage.Create(api);
                page7.Id = PAGE_7_ID;
                page7.SiteId = SITE_ID;
                page7.Title = "My base page";
                page7.Ingress = "My base ingress";
                page7.Body = "My base body";
                page7.ParentId = PAGE_1_ID;
                page7.SortOrder = 1;
                api.Pages.Save(page7);

                var page8 = MyPage.Create(api);
                page8.OriginalPageId = PAGE_7_ID;
                page8.Id = PAGE_8_ID;
                page8.SiteId = SITE_ID;
                page8.Title = "My copied page";
                page8.ParentId = PAGE_1_ID;
                page8.SortOrder = 2;
                page8.IsHidden = true;
                page8.Route = "test-route";

                api.Pages.Save(page8);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var pages = api.Pages.GetAll(SITE_ID);

                foreach (var page in pages.Where(p => p.OriginalPageId.HasValue))
                    api.Pages.Delete(page);
                foreach (var page in pages.Where(p => p.ParentId.HasValue))
                    api.Pages.Delete(page);
                foreach (var page in pages.Where(p => !p.ParentId.HasValue))
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
        public void IsCached() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(PagesCached), api.IsCached);
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Pages.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Pages.GetBySlug("none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetStartpage() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetStartpage();

                Assert.NotNull(model);
                Assert.Null(model.ParentId);
                Assert.Equal(0, model.SortOrder);
            }
        }

        [Fact]
        public void GetStartpageBySite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetStartpage(SITE_ID);

                Assert.NotNull(model);
                Assert.Null(model.ParentId);
                Assert.Equal(0, model.SortOrder);
            }
        }

        [Fact]
        public void GetStartpageNone() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetStartpage(SITE_EMPTY_ID);

                Assert.Null(model);
            }
        }

        [Fact]
        public void GetIdBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetIdBySlug("my-first-page");

                Assert.NotNull(model);
                Assert.Equal(PAGE_1_ID, model.Value);
            }
        }

        [Fact]
        public void GetIdBySlugSiteId() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetIdBySlug("my-first-page", SITE_ID);

                Assert.NotNull(model);
                Assert.Equal(PAGE_1_ID, model.Value);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var pages = api.Pages.GetAll(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }
        }

        [Fact]
        public void GetAllByBaseClass() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var pages = api.Pages.GetAll<Models.PageBase>(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }
        }

        [Fact]
        public void GetAllBlogs() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var pages = api.Pages.GetAllBlogs(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }            
        }

        [Fact]
        public void GetAllBlogsByBaseClass() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var pages = api.Pages.GetAllBlogs<MyBlogPage>(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }            
        }

        [Fact]
        public void GetAllByMissing() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var pages = api.Pages.GetAll<MissingPage>(SITE_ID);

                Assert.NotNull(pages);
                Assert.Empty(pages);
            }
        }        

        [Fact]
        public void GetGenericById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById<MyPage>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetBaseClassById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById<Models.PageBase>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPage), model.GetType());
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", ((MyPage)model).Body.Value);
            }
        }

        [Fact]
        public void GetBlocksById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById<MyPage>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal(2, model.Blocks.Count);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[0]);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[1]);
            }            
        }

        [Fact]
        public void GetMissingById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById<MissingPage>(PAGE_1_ID);

                Assert.Null(model);
            }
        }

        [Fact]
        public void GetInfoById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById<Models.PageInfo>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Empty(model.Blocks);
            }            
        }

        [Fact]
        public void GetGenericBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetBySlug<MyPage>("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetBaseClassBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetBySlug<Models.PageBase>("my-first-page");

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPage), model.GetType());
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", ((MyPage)model).Body.Value);
            }
        }

        [Fact]
        public void GetMissingBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetBySlug<MissingPage>("my-first-page");

                Assert.Null(model);
            }
        }

        [Fact]
        public void GetInfoBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetBySlug<Models.PageInfo>("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Empty(model.Blocks);
            }
        }        

        [Fact]
        public void GetDynamicById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetBySlug("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("My first page", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void CheckPermlinkSyntax() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.NotNull(model.Permalink);
                Assert.StartsWith("/", model.Permalink);
            }
        }

        [Fact]
        public void GetCollectionPage() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetBySlug<MyCollectionPage>("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(3, page.Texts.Count);
                Assert.Equal("Second text", page.Texts[1].Value);
            }
        }

        [Fact]
        public void GetCollectionPageBaseClass() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetBySlug<Models.PageBase>("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(typeof(MyCollectionPage), page.GetType());
                Assert.Equal(3, ((MyCollectionPage)page).Texts.Count);
                Assert.Equal("Second text", ((MyCollectionPage)page).Texts[1].Value);
            }
        }

        [Fact]
        public void GetDynamicCollectionPage() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetBySlug("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(3, page.Regions.Texts.Count);
                Assert.Equal("Second text", page.Regions.Texts[1].Value);
            }
        }

        [Fact]
        public void EmptyCollectionPage() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = MyCollectionPage.Create(api);

                Assert.Equal(0, page.Texts.Count);

                page.SiteId = SITE_ID;
                page.Title = "Another collection page";

                api.Pages.Save(page);

                page = api.Pages.GetBySlug<MyCollectionPage>(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Texts.Count);
            }
        }

        [Fact]
        public void EmptyDynamicCollectionPage() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = Piranha.Models.DynamicPage.Create(api, "MyCollectionPage");

                Assert.Equal(0, page.Regions.Texts.Count);

                page.SiteId = SITE_ID;
                page.Title = "Third collection page";

                api.Pages.Save(page);

                page = api.Pages.GetBySlug(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Regions.Texts.Count);
            }
        }

        [Fact]
        public void EmptyCollectionPageComplex() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = MyCollectionPage.Create(api);

                Assert.Equal(0, page.Teasers.Count);

                page.SiteId = SITE_ID;
                page.Title = "Fourth collection page";

                api.Pages.Save(page);

                page = api.Pages.GetBySlug<MyCollectionPage>(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Teasers.Count);
            }
        }

        [Fact]
        public void EmptyDynamicCollectionPageComplex() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = Piranha.Models.DynamicPage.Create(api, "MyCollectionPage");

                Assert.Equal(0, page.Regions.Teasers.Count);

                page.SiteId = SITE_ID;
                page.Title = "Fifth collection page";

                api.Pages.Save(page);

                page = api.Pages.GetBySlug(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Regions.Teasers.Count);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var count = api.Pages.GetAll(SITE_ID).Count();
                var page = MyPage.Create(api, "MyPage");
                page.SiteId = SITE_ID;
                page.Title = "My fourth page";
                page.Ingress = "My fourth ingress";
                page.Body = "My fourth body";

                api.Pages.Save(page);

                Assert.Equal(count + 1, api.Pages.GetAll(SITE_ID).Count());
            }
        }

        [Fact]
        public void AddHierarchical() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                using (var config = new Piranha.Config(api)) {
                    config.HierarchicalPageSlugs = true;
                }

                var page = MyPage.Create(api, "MyPage");
                page.Id = Guid.NewGuid();
                page.ParentId = PAGE_1_ID;
                page.SiteId = SITE_ID;
                page.Title = "My subpage";
                page.Ingress = "My subpage ingress";
                page.Body = "My subpage body";

                api.Pages.Save(page);
                
                page = api.Pages.GetById<MyPage>(page.Id);

                Assert.NotNull(page);
                Assert.Equal("my-first-page/my-subpage", page.Slug);

                var param = api.Params.GetByKey(Piranha.Config.PAGES_HIERARCHICAL_SLUGS);
                api.Params.Delete(param);
            }            
        }

        [Fact]
        public void AddNonHierarchical() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                using (var config = new Piranha.Config(api)) {
                    config.HierarchicalPageSlugs = false;
                }

                var page = MyPage.Create(api, "MyPage");
                page.Id = Guid.NewGuid();
                page.ParentId = PAGE_1_ID;
                page.SiteId = SITE_ID;
                page.Title = "My second subpage";
                page.Ingress = "My subpage ingress";
                page.Body = "My subpage body";

                api.Pages.Save(page);
                
                page = api.Pages.GetById<MyPage>(page.Id);

                Assert.NotNull(page);
                Assert.Equal("my-second-subpage", page.Slug);

                var param = api.Params.GetByKey(Piranha.Config.PAGES_HIERARCHICAL_SLUGS);
                api.Params.Delete(param);
            }            
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetById<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal("My first page", page.Title);

                page.Title = "Updated page";
                page.IsHidden = true;
                api.Pages.Save(page);

                page = api.Pages.GetById<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal("Updated page", page.Title);
                Assert.True(page.IsHidden);
            }
        }

        [Fact]
        public void UpdateCollectionPage() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetBySlug<MyCollectionPage>("my-collection-page", SITE_ID);

                Assert.NotNull(page);
                Assert.Equal(3, page.Texts.Count);
                Assert.Equal("First text", page.Texts[0].Value);

                page.Texts[0] = "Updated text";
                page.Texts.RemoveAt(2);
                api.Pages.Save(page);

                page = api.Pages.GetBySlug<MyCollectionPage>("my-collection-page", SITE_ID);
                
                Assert.NotNull(page);
                Assert.Equal(2, page.Texts.Count);
                Assert.Equal("Updated text", page.Texts[0].Value);
            }
        }

        [Fact]
        public void Move() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetById<MyPage>(PAGE_3_ID);
                var count = api.Pages.GetAll(SITE_ID).Count();

                Assert.NotNull(page);

                api.Pages.Delete(page);

                Assert.Equal(count - 1, api.Pages.GetAll(SITE_ID).Count());
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var count = api.Pages.GetAll(SITE_ID).Count();

                api.Pages.Delete(PAGE_2_ID);

                Assert.Equal(count - 1, api.Pages.GetAll(SITE_ID).Count());
            }
        }

        [Fact]
        public void GetDIGeneric() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetById<MyDIPage>(PAGE_DI_ID);

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Body.Value);
            }            
        }

        [Fact]
        public void GetDIDynamic() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetById(PAGE_DI_ID);

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Regions.Body.Value);
            }            
        }

        [Fact]
        public void CreateDIGeneric() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = MyDIPage.Create(api);

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Body.Value);
            }            
        }

        [Fact]
        public void CreateDIDynamic() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = Models.DynamicPage.Create(api, nameof(MyDIPage));

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Regions.Body.Value);
            }            
        }

        [Fact]
        public void GetCopyGenericById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetById<MyPage>(PAGE_8_ID);

                Assert.NotNull(model);
                Assert.Equal("My copied page", model.Title);
                Assert.Equal("my-first-page/my-copied-page", model.Slug);
                Assert.Equal(PAGE_1_ID, model.ParentId);
                Assert.Equal(2, model.SortOrder);
                Assert.True(model.IsHidden);
                Assert.Equal("test-route", model.Route);

                Assert.Equal(PAGE_7_ID, model.OriginalPageId);
                Assert.Equal("My base body", model.Body.Value);
            }
        }

        [Fact]
        public void GetCopyGenericBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Pages.GetBySlug<MyPage>("my-first-page/my-copied-page");

                Assert.NotNull(model);
                Assert.Equal("My copied page", model.Title);
                Assert.Equal("my-first-page/my-copied-page", model.Slug);
                Assert.Equal(PAGE_1_ID, model.ParentId);
                Assert.Equal(2, model.SortOrder);
                Assert.True(model.IsHidden);
                Assert.Equal("test-route", model.Route);

                Assert.Equal(PAGE_7_ID, model.OriginalPageId);
                Assert.Equal("My base body", model.Body.Value);
            }
            
        }

        [Fact]
        public void UpdatingCopyShouldIgnoreBodyAndDate() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = api.Pages.GetById<MyPage>(PAGE_8_ID);
                page.Created = DateTime.Parse("2001-01-01");
                page.LastModified = DateTime.Parse("2001-01-01");
                page.Body = "My edits to the body";

                api.Pages.Save(page);
                page = api.Pages.GetById<MyPage>(PAGE_8_ID);

                Assert.NotEqual(DateTime.Parse("2001-01-01"), page.Created);
                Assert.NotEqual(DateTime.Parse("2001-01-01"), page.LastModified);
                Assert.NotEqual("My edits to the body", page.Body.ToString());
            }
        }

        [Fact]
        public void CanNotUpdateCopyOriginalPageWithAnotherCopy() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = MyPage.Create(api);
                page.Title = "New title";
                page.OriginalPageId = PAGE_8_ID; // PAGE_8 is an copy of PAGE_7

                var exn = Assert.Throws<Exception>(() => {
                    api.Pages.Save(page);
                });

                Assert.Equal("Can not set copy of a copy", exn.Message);
            }
        }

        [Fact]
        public void CanNotUpdateCopyWithAnotherTypeIdOtherThanOriginalPageTypeId() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var page = MissingPage.Create(api);
                page.Title = "New title";
                page.OriginalPageId = PAGE_7_ID;

                var exn = Assert.Throws<Exception>(() => {
                    api.Pages.Save(page);
                });

                Assert.Equal("Copy can not have a different content type", exn.Message);
            }
        }

        [Fact]
        public void DetachShouldCopyBlocks() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var originalPage = api.Pages.GetById<MyPage>(PAGE_7_ID);
                var copy = api.Pages.GetById<MyPage>(PAGE_8_ID);
                var originalBlock = new Extend.Blocks.TextBlock {
                    Id = Guid.NewGuid(),
                    Body = "test",
                };

                originalPage.Blocks.Add(originalBlock);
                api.Pages.Save(originalPage);

                api.Pages.Detach(copy);

                var p = api.Pages.GetById<MyPage>(PAGE_8_ID);
                Assert.Collection(p.Blocks, e => {
                    Assert.NotEqual(e.Id, originalBlock.Id);
                    var eBlock = Assert.IsType<Extend.Blocks.TextBlock>(e);
                    Assert.Equal(eBlock.Body.Value, originalBlock.Body.Value);
                });
            }
        }

        [Fact]
        public void DetachShouldCopyRegions() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var originalPage = api.Pages.GetById<MyPage>(PAGE_7_ID);
                originalPage.Body = "body to be copied";
                originalPage.Ingress = "ingress to be copied";
                api.Pages.Save(originalPage);

                var copy = api.Pages.GetById<MyPage>(PAGE_8_ID);
                api.Pages.Detach(copy);

                originalPage = api.Pages.GetById<MyPage>(PAGE_7_ID);
                originalPage.Body = "body should not be copied";
                originalPage.Ingress = "ingress should not be copied";
                api.Pages.Save(originalPage);

                var p = api.Pages.GetById<MyPage>(PAGE_8_ID);
                Assert.Equal("body to be copied", p.Body.Value);
                Assert.Equal("ingress to be copied", p.Ingress.Value);
            }
        }

        [Fact]
        public void DeleteShouldThrowWhenPageHasCopies() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var exn = Assert.Throws<Exception>(() => {
                    api.Pages.Delete(PAGE_7_ID);
                });
                Assert.Equal("Can not delete page because it has copies", exn.Message);
            }
        }
    }
}
