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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class PageTestsCached : PageTests
    {
        public override async Task InitializeAsync()
        {
            _cache = new Cache.SimpleCache();

            await base.InitializeAsync();
        }
    }

    [Collection("Integration tests")]
    public class PageTests : BaseTestsAsync
    {
        public readonly Guid SITE_ID = Guid.NewGuid();
        public readonly Guid SITE_EMPTY_ID = Guid.NewGuid();
        public readonly Guid PAGE_1_ID = Guid.NewGuid();
        public readonly Guid PAGE_2_ID = Guid.NewGuid();
        public readonly Guid PAGE_3_ID = Guid.NewGuid();
        public readonly Guid PAGE_7_ID = Guid.NewGuid();
        public readonly Guid PAGE_8_ID = Guid.NewGuid();
        public readonly Guid PAGE_DI_ID = Guid.NewGuid();

        public interface IMyService
        {
            string Value { get; }
        }

        public class MyService : IMyService
        {
            public string Value { get; private set; } = "My service value";
        }

        [Piranha.Extend.FieldType(Name = "Fourth")]
        public class MyFourthField : Extend.Fields.SimpleField<string>
        {
            public void Init(IMyService myService)
            {
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

        [PageType(Title = "My BlogType", IsArchive = true)]
        public class MyBlogPage : Models.Page<MyBlogPage>
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
            public IList<TextField> Texts { get; set; } = new List<TextField>();
            [Region]
            public IList<ComplexRegion> Teasers { get; set; } = new List<ComplexRegion>();
        }

        [PageType(Title = "Injection PageType")]
        public class MyDIPage : Models.Page<MyDIPage>
        {
            [Region]
            public MyFourthField Body { get; set; }
        }

        public override async Task InitializeAsync()
        {
            _services = new ServiceCollection()
                .AddSingleton<IMyService, MyService>()
                .BuildServiceProvider();

            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                Piranha.App.Fields.Register<MyFourthField>();

                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(MissingPage))
                    .AddType(typeof(MyBlogPage))
                    .AddType(typeof(MyPage))
                    .AddType(typeof(MyCollectionPage))
                    .AddType(typeof(MyDIPage));
                builder.Build();

                var site = new Site
                {
                    Id = SITE_ID,
                    Title = "My Test Site",
                    InternalId = "MyTestSite",
                    IsDefault = true
                };
                await api.Sites.SaveAsync(site);

                var page1 = await MyPage.CreateAsync(api);
                page1.Id = PAGE_1_ID;
                page1.SiteId = SITE_ID;
                page1.Title = "My first page";
                page1.Ingress = "My first ingress";
                page1.Body = "My first body";
                page1.Blocks.Add(new Extend.Blocks.TextBlock
                {
                    Body = "Sollicitudin Aenean"
                });
                page1.Blocks.Add(new Extend.Blocks.TextBlock
                {
                    Body = "Ipsum Elit"
                });
                page1.Published = DateTime.Now;
                await api.Pages.SaveAsync(page1);

                var page2 = await MyPage.CreateAsync(api);
                page2.Id = PAGE_2_ID;
                page2.SiteId = SITE_ID;
                page2.Title = "My second page";
                page2.Ingress = "My second ingress";
                page2.Body = "My second body";
                await api.Pages.SaveAsync(page2);

                var page3 = await MyPage.CreateAsync(api);
                page3.Id = PAGE_3_ID;
                page3.SiteId = SITE_ID;
                page3.Title = "My third page";
                page3.Ingress = "My third ingress";
                page3.Body = "My third body";
                await api.Pages.SaveAsync(page3);

                var page4 = await MyCollectionPage.CreateAsync(api);
                page4.SiteId = SITE_ID;
                page4.Title = "My collection page";
                page4.SortOrder = 1;
                page4.Texts.Add(new TextField
                {
                    Value = "First text"
                });
                page4.Texts.Add(new TextField
                {
                    Value = "Second text"
                });
                page4.Texts.Add(new TextField
                {
                    Value = "Third text"
                });
                await api.Pages.SaveAsync(page4);

                var page5 = await MyBlogPage.CreateAsync(api);
                page5.SiteId = SITE_ID;
                page5.Title = "Blog Archive";
                await api.Pages.SaveAsync(page5);

                var page6 = await MyDIPage.CreateAsync(api);
                page6.Id = PAGE_DI_ID;
                page6.SiteId = SITE_ID;
                page6.Title = "My Injection Page";
                await api.Pages.SaveAsync(page6);

                var page7 = await MyPage.CreateAsync(api);
                page7.Id = PAGE_7_ID;
                page7.SiteId = SITE_ID;
                page7.Title = "My base page";
                page7.Ingress = "My base ingress";
                page7.Body = "My base body";
                page7.ParentId = PAGE_1_ID;
                page7.SortOrder = 1;
                await api.Pages.SaveAsync(page7);

                var page8 = await MyPage.CreateAsync(api);
                page8.OriginalPageId = PAGE_7_ID;
                page8.Id = PAGE_8_ID;
                page8.SiteId = SITE_ID;
                page8.Title = "My copied page";
                page8.ParentId = PAGE_1_ID;
                page8.SortOrder = 2;
                page8.IsHidden = true;
                page8.Route = "test-route";

                await api.Pages.SaveAsync(page8);
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllAsync(SITE_ID);

                foreach (var page in pages.Where(p => p.OriginalPageId.HasValue))
                {
                    await api.Pages.DeleteAsync(page);
                }
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

                var site = await api.Sites.GetByIdAsync(SITE_ID);
                if (site != null)
                {
                    await api.Sites.DeleteAsync(site);
                }
            }
        }

        [Fact]
        public void IsCached()
        {
            using (var api = CreateApi())
            {
                Assert.Equal(this.GetType() == typeof(PageTestsCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public async Task GetNoneById()
        {
            using (var api = CreateApi())
            {
                var none = await api.Pages.GetByIdAsync(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneBySlug()
        {
            using (var api = CreateApi())
            {
                var none = await api.Pages.GetBySlugAsync("none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetStartpage()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetStartpageAsync();

                Assert.NotNull(model);
                Assert.Null(model.ParentId);
                Assert.Equal(0, model.SortOrder);
            }
        }

        [Fact]
        public async Task GetStartpageBySite()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetStartpageAsync(SITE_ID);

                Assert.NotNull(model);
                Assert.Null(model.ParentId);
                Assert.Equal(0, model.SortOrder);
            }
        }

        [Fact]
        public async Task GetStartpageNone()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetStartpageAsync(SITE_EMPTY_ID);

                Assert.Null(model);
            }
        }

        [Fact]
        public async Task GetIdBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetIdBySlugAsync("my-first-page");

                Assert.NotNull(model);
                Assert.Equal(PAGE_1_ID, model.Value);
            }
        }

        [Fact]
        public async Task GetIdBySlugSiteId()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetIdBySlugAsync("my-first-page", SITE_ID);

                Assert.NotNull(model);
                Assert.Equal(PAGE_1_ID, model.Value);
            }
        }

        [Fact]
        public async Task GetAll()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllAsync(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }
        }

        [Fact]
        public async Task GetAllByBaseClass()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllAsync<Models.PageBase>(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }
        }

        [Fact]
        public async Task GetAllBlogs()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllBlogsAsync(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }
        }

        [Fact]
        public async Task GetAllBlogsByBaseClass()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllBlogsAsync<MyBlogPage>(SITE_ID);

                Assert.NotNull(pages);
                Assert.NotEmpty(pages);
            }
        }

        [Fact]
        public async Task GetAllByMissing()
        {
            using (var api = CreateApi())
            {
                var pages = await api.Pages.GetAllAsync<MissingPage>(SITE_ID);

                Assert.NotNull(pages);
                Assert.Empty(pages);
            }
        }

        [Fact]
        public async Task GetGenericById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync<MyPage>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public async Task GetBaseClassById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync<Models.PageBase>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPage), model.GetType());
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", ((MyPage)model).Body.Value);
            }
        }

        [Fact]
        public async Task GetBlocksById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync<MyPage>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal(2, model.Blocks.Count);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[0]);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[1]);
            }
        }

        [Fact]
        public async Task GetMissingById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync<MissingPage>(PAGE_1_ID);

                Assert.Null(model);
            }
        }

        [Fact]
        public async Task GetInfoById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync<Models.PageInfo>(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Empty(model.Blocks);
            }
        }

        [Fact]
        public async Task GetGenericBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetBySlugAsync<MyPage>("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public async Task GetBaseClassBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetBySlugAsync<Models.PageBase>("my-first-page");

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPage), model.GetType());
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", ((MyPage)model).Body.Value);
            }
        }

        [Fact]
        public async Task GetMissingBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetBySlugAsync<MissingPage>("my-first-page");

                Assert.Null(model);
            }
        }

        [Fact]
        public async Task GetInfoBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetBySlugAsync<Models.PageInfo>("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Empty(model.Blocks);
            }
        }

        [Fact]
        public async Task GetDynamicById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-page", model.Slug);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public async Task GetDynamicBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetBySlugAsync("my-first-page");

                Assert.NotNull(model);
                Assert.Equal("My first page", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public async Task CheckPermlinkSyntax()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync(PAGE_1_ID);

                Assert.NotNull(model);
                Assert.NotNull(model.Permalink);
                Assert.StartsWith("/", model.Permalink);
            }
        }

        [Fact]
        public async Task GetCollectionPage()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetBySlugAsync<MyCollectionPage>("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(3, page.Texts.Count);
                Assert.Equal("Second text", page.Texts[1].Value);
            }
        }

        [Fact]
        public async Task GetCollectionPageBaseClass()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetBySlugAsync<Models.PageBase>("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(typeof(MyCollectionPage), page.GetType());
                Assert.Equal(3, ((MyCollectionPage)page).Texts.Count);
                Assert.Equal("Second text", ((MyCollectionPage)page).Texts[1].Value);
            }
        }

        [Fact]
        public async Task GetDynamicCollectionPage()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetBySlugAsync("my-collection-page");

                Assert.NotNull(page);
                Assert.Equal(3, page.Regions.Texts.Count);
                Assert.Equal("Second text", page.Regions.Texts[1].Value);
            }
        }

        [Fact]
        public async Task EmptyCollectionPage()
        {
            using (var api = CreateApi())
            {
                var page = await MyCollectionPage.CreateAsync(api);

                Assert.Equal(0, page.Texts.Count);

                page.SiteId = SITE_ID;
                page.Title = "Another collection page";

                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetBySlugAsync<MyCollectionPage>(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Texts.Count);
            }
        }

        [Fact]
        public async Task EmptyDynamicCollectionPage()
        {
            using (var api = CreateApi())
            {
                var page = await Piranha.Models.DynamicPage.CreateAsync(api, "MyCollectionPage");

                Assert.Equal(0, page.Regions.Texts.Count);

                page.SiteId = SITE_ID;
                page.Title = "Third collection page";

                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetBySlugAsync(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Regions.Texts.Count);
            }
        }

        [Fact]
        public async Task EmptyCollectionPageComplex()
        {
            using (var api = CreateApi())
            {
                var page = await MyCollectionPage.CreateAsync(api);

                Assert.Equal(0, page.Teasers.Count);

                page.SiteId = SITE_ID;
                page.Title = "Fourth collection page";

                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetBySlugAsync<MyCollectionPage>(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Teasers.Count);
            }
        }

        [Fact]
        public async Task EmptyDynamicCollectionPageComplex()
        {
            using (var api = CreateApi())
            {
                var page = await Piranha.Models.DynamicPage.CreateAsync(api, "MyCollectionPage");

                Assert.Equal(0, page.Regions.Teasers.Count);

                page.SiteId = SITE_ID;
                page.Title = "Fifth collection page";

                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetBySlugAsync(Piranha.Utils.GenerateSlug(page.Title), SITE_ID);

                Assert.Equal(0, page.Regions.Teasers.Count);
            }
        }

        [Fact]
        public async Task Add()
        {
            using (var api = CreateApi())
            {
                var count = (await api.Pages.GetAllAsync(SITE_ID)).Count();
                var page = await MyPage.CreateAsync(api, "MyPage");
                page.SiteId = SITE_ID;
                page.Title = "My fourth page";
                page.Ingress = "My fourth ingress";
                page.Body = "My fourth body";

                await api.Pages.SaveDraftAsync(page);

                Assert.Equal(count + 1, (await api.Pages.GetAllAsync(SITE_ID)).Count());
            }
        }

        [Fact]
        public async Task AddHierarchical()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.HierarchicalPageSlugs = true;
                }

                var page = await MyPage.CreateAsync(api, "MyPage");
                page.Id = Guid.NewGuid();
                page.ParentId = PAGE_1_ID;
                page.SiteId = SITE_ID;
                page.Title = "My subpage";
                page.Ingress = "My subpage ingress";
                page.Body = "My subpage body";

                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetByIdAsync<MyPage>(page.Id);

                Assert.NotNull(page);
                Assert.Equal("my-first-page/my-subpage", page.Slug);

                var param = await api.Params.GetByKeyAsync(Piranha.Config.PAGES_HIERARCHICAL_SLUGS);
                await api.Params.DeleteAsync(param);
            }
        }

        [Fact]
        public async Task AddNonHierarchical()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.HierarchicalPageSlugs = false;
                }

                var page = await MyPage.CreateAsync(api, "MyPage");
                page.Id = Guid.NewGuid();
                page.ParentId = PAGE_1_ID;
                page.SiteId = SITE_ID;
                page.Title = "My second subpage";
                page.Ingress = "My subpage ingress";
                page.Body = "My subpage body";

                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetByIdAsync<MyPage>(page.Id);

                Assert.NotNull(page);
                Assert.Equal("my-second-subpage", page.Slug);

                var param = await api.Params.GetByKeyAsync(Piranha.Config.PAGES_HIERARCHICAL_SLUGS);
                await api.Params.DeleteAsync(param);
            }
        }

        [Fact]
        public async Task Update()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetByIdAsync<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal("My first page", page.Title);

                page.Title = "Updated page";
                page.IsHidden = true;
                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetByIdAsync<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal("Updated page", page.Title);
                Assert.True(page.IsHidden);
            }
        }

        [Fact]
        public async Task SaveDraft()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetByIdAsync<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);

                page.Title = "My working copy";
                await api.Pages.SaveDraftAsync(page);

                page = await api.Pages.GetByIdAsync<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.NotEqual("My working copy", page.Title);

                page = await api.Pages.GetDraftByIdAsync<MyPage>(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal("My working copy", page.Title);
            }
        }

        [Fact]
        public async Task UpdateCollectionPage()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetBySlugAsync<MyCollectionPage>("my-collection-page", SITE_ID);

                Assert.NotNull(page);
                Assert.Equal(3, page.Texts.Count);
                Assert.Equal("First text", page.Texts[0].Value);

                page.Texts[0] = "Updated text";
                page.Texts.RemoveAt(2);
                await api.Pages.SaveAsync(page);

                page = await api.Pages.GetBySlugAsync<MyCollectionPage>("my-collection-page", SITE_ID);

                Assert.NotNull(page);
                Assert.Equal(2, page.Texts.Count);
                Assert.Equal("Updated text", page.Texts[0].Value);
            }
        }

        [Fact]
        public async Task Move()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetByIdAsync(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.True(page.SortOrder > 0);

                page.SortOrder = 0;
                await api.Pages.MoveAsync(page, null, 0);

                page = await api.Pages.GetByIdAsync(PAGE_1_ID);

                Assert.NotNull(page);
                Assert.Equal(0, page.SortOrder);
            }
        }

        [Fact]
        public async Task Delete()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetByIdAsync<MyPage>(PAGE_3_ID);
                var count = (await api.Pages.GetAllAsync(SITE_ID)).Count();

                Assert.NotNull(page);

                await api.Pages.DeleteAsync(page);

                Assert.Equal(count - 1, (await api.Pages.GetAllAsync(SITE_ID)).Count());
            }
        }

        [Fact]
        public async Task DeleteById()
        {
            using (var api = CreateApi())
            {
                var count = (await api.Pages.GetAllAsync(SITE_ID)).Count();

                await api.Pages.DeleteAsync(PAGE_2_ID);

                Assert.Equal(count - 1, (await api.Pages.GetAllAsync(SITE_ID)).Count());
            }
        }

        [Fact]
        public async Task GetDIGeneric()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetByIdAsync<MyDIPage>(PAGE_DI_ID);

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Body.Value);
            }
        }

        [Fact]
        public async Task GetDIDynamic()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetByIdAsync(PAGE_DI_ID);

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Regions.Body.Value);
            }
        }

        [Fact]
        public async Task CreateDIGeneric()
        {
            using (var api = CreateApi())
            {
                var page = await MyDIPage.CreateAsync(api);

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Body.Value);
            }
        }

        [Fact]
        public async Task CreateDIDynamic()
        {
            using (var api = CreateApi())
            {
                var page = await Models.DynamicPage.CreateAsync(api, nameof(MyDIPage));

                Assert.NotNull(page);
                Assert.Equal("My service value", page.Regions.Body.Value);
            }
        }

        [Fact]
        public async Task GetCopyGenericById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetByIdAsync<MyPage>(PAGE_8_ID);

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
        public async Task GetCopyGenericBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Pages.GetBySlugAsync<MyPage>("my-first-page/my-copied-page");

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
        public async Task UpdatingCopyShouldIgnoreBodyAndDate()
        {
            using (var api = CreateApi())
            {
                var page = await api.Pages.GetByIdAsync<MyPage>(PAGE_8_ID);
                page.Created = DateTime.Parse("2001-01-01");
                page.LastModified = DateTime.Parse("2001-01-01");
                page.Body = "My edits to the body";

                await api.Pages.SaveAsync(page);
                page = await api.Pages.GetByIdAsync<MyPage>(PAGE_8_ID);

                Assert.NotEqual(DateTime.Parse("2001-01-01"), page.Created);
                Assert.NotEqual(DateTime.Parse("2001-01-01"), page.LastModified);
                Assert.NotEqual("My edits to the body", page.Body.ToString());
            }
        }

        [Fact]
        public async Task CanNotUpdateCopyOriginalPageWithAnotherCopy()
        {
            using (var api = CreateApi())
            {
                var page = await MyPage.CreateAsync(api);
                page.Title = "New title";
                page.OriginalPageId = PAGE_8_ID; // PAGE_8 is an copy of PAGE_7

                var exn = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                {
                    await api.Pages.SaveAsync(page);
                });

                Assert.Equal("Can not set copy of a copy", exn.Message);
            }
        }

        [Fact]
        public async Task CanNotUpdateCopyWithAnotherTypeIdOtherThanOriginalPageTypeId()
        {
            using (var api = CreateApi())
            {
                var page = await MissingPage.CreateAsync(api);
                page.Title = "New title";
                page.OriginalPageId = PAGE_7_ID;

                var exn = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                {
                    await api.Pages.SaveAsync(page);
                });

                Assert.Equal("Copy can not have a different content type", exn.Message);
            }
        }

        [Fact]
        public async Task DetachShouldCopyBlocks()
        {
            using (var api = CreateApi())
            {
                var originalPage = await api.Pages.GetByIdAsync<MyPage>(PAGE_7_ID);
                var copy = await api.Pages.GetByIdAsync<MyPage>(PAGE_8_ID);
                var originalBlock = new Extend.Blocks.TextBlock
                {
                    Id = Guid.NewGuid(),
                    Body = "test",
                };

                originalPage.Blocks.Add(originalBlock);
                await api.Pages.SaveAsync(originalPage);

                await api.Pages.DetachAsync(copy);

                var p = await api.Pages.GetByIdAsync<MyPage>(PAGE_8_ID);
                Assert.Collection(p.Blocks, e =>
                {
                    Assert.NotEqual(e.Id, originalBlock.Id);
                    var eBlock = Assert.IsType<Extend.Blocks.TextBlock>(e);
                    Assert.Equal(eBlock.Body.Value, originalBlock.Body.Value);
                });
            }
        }

        [Fact]
        public async Task DetachShouldCopyRegions()
        {
            using (var api = CreateApi())
            {
                var originalPage = await api.Pages.GetByIdAsync<MyPage>(PAGE_7_ID);
                originalPage.Body = "body to be copied";
                originalPage.Ingress = "ingress to be copied";
                await api.Pages.SaveAsync(originalPage);

                var copy = await api.Pages.GetByIdAsync<MyPage>(PAGE_8_ID);
                await api.Pages.DetachAsync(copy);

                originalPage = await api.Pages.GetByIdAsync<MyPage>(PAGE_7_ID);
                originalPage.Body = "body should not be copied";
                originalPage.Ingress = "ingress should not be copied";
                await api.Pages.SaveAsync(originalPage);

                var p = await api.Pages.GetByIdAsync<MyPage>(PAGE_8_ID);
                Assert.Equal("body to be copied", p.Body.Value);
                Assert.Equal("ingress to be copied", p.Ingress.Value);
            }
        }

        [Fact]
        public async Task DeleteShouldThrowWhenPageHasCopies()
        {
            using (var api = CreateApi())
            {
                var exn = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                {
                    await api.Pages.DeleteAsync(PAGE_7_ID);
                });
                Assert.Equal("Can not delete page because it has copies", exn.Message);
            }
        }
    }
}
