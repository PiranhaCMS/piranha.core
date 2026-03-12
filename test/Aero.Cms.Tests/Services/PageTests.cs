

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Aero.Cms.AttributeBuilder;
using Aero.Cms.Extend;
using Aero.Cms.Extend.Fields;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

[Collection("Integration tests")]
public class PageTestsMemoryCache : PageTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

[Collection("Integration tests")]
public class PageTestsDistributedCache : PageTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

[Collection("Integration tests")]
public class PageTests : BaseTestsAsync
{
    public readonly string SITEID = Snowflake.NewId();
    public readonly string SITEEMPTYID = Snowflake.NewId();
    public readonly string PAGE1ID = Snowflake.NewId();
    public readonly string PAGE2ID = Snowflake.NewId();
    public readonly string PAGE3ID = Snowflake.NewId();
    public readonly string PAGE7ID = Snowflake.NewId();
    public readonly string PAGE8ID = Snowflake.NewId();
    public readonly string PAGEDIID = Snowflake.NewId();

    [Aero.Cms.Extend.FieldType(Name = "Fourth")]
    public class MyFourthField : Extend.Fields.SimpleField<string>
    {
        public void Init(IMyService myService)
        {
            Value = myService.Value;
        }
    }

    public class ComplexRegion
    {
        [Field] public StringField Title { get; set; }
        [Field] public TextField Body { get; set; }
    }

    [PageType(Title = "My PageType")]
    public class MyPage : Models.Page<MyPage>
    {
        [Region] public TextField Ingress { get; set; }
        [Region] public MarkdownField Body { get; set; }
    }

    [PageType(Title = "My BlogType", IsArchive = true)]
    public class MyBlogPage : Models.Page<MyBlogPage>
    {
        [Region] public TextField Ingress { get; set; }
        [Region] public MarkdownField Body { get; set; }
    }

    [PageType(Title = "Missing PageType")]
    public class MissingPage : Models.Page<MissingPage>
    {
        [Region] public TextField Ingress { get; set; }
        [Region] public MarkdownField Body { get; set; }
    }

    [PageType(Title = "My CollectionPage")]
    public class MyCollectionPage : Models.Page<MyCollectionPage>
    {
        [Region] public List<TextField> Texts { get; set; } = new List<TextField>();
        [Region] public List<ComplexRegion> Teasers { get; set; } = new List<ComplexRegion>();
    }

    [PageType(Title = "Injection PageType")]
    public class MyDIPage : Models.Page<MyDIPage>
    {
        [Region] public MyFourthField Body { get; set; }
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        // Func<IServiceCollection, IServiceCollection> registration = sc => sc
        //     .AddSingleton<IMyService, MyService>();
        // replace default services w/ custom registrations 
        services = CreateServiceCollection(store)
            .BuildServiceProvider();

        using var api = CreateApi();
        Aero.Cms.App.Init(api);

        Aero.Cms.App.Fields.Register<MyFourthField>();

        new ContentTypeBuilder(api)
            .AddType(typeof(MissingPage))
            .AddType(typeof(MyBlogPage))
            .AddType(typeof(MyPage))
            .AddType(typeof(MyCollectionPage))
            .AddType(typeof(MyDIPage))
            .Build();

        var site = new Site
        {
            Id = SITEID,
            Title = "My Test Site",
            InternalId = "MyTestSite",
            IsDefault = true
        };
        await api.Sites.SaveAsync(site);

        var page1 = await MyPage.CreateAsync(api);
        page1.Id = PAGE1ID;
        page1.SiteId = SITEID;
        page1.Title = "My first page";
        page1.MetaKeywords = "Keywords";
        page1.MetaDescription = "Description";
        page1.OgTitle = "Og Title";
        page1.OgDescription = "Og Description";
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
        page2.Id = PAGE2ID;
        page2.SiteId = SITEID;
        page2.Title = "My second page";
        page2.MetaFollow = false;
        page2.MetaIndex = false;
        page2.Ingress = "My second ingress";
        page2.Body = "My second body";
        await api.Pages.SaveAsync(page2);

        var page3 = await MyPage.CreateAsync(api);
        page3.Id = PAGE3ID;
        page3.SiteId = SITEID;
        page3.Title = "My third page";
        page3.Ingress = "My third ingress";
        page3.Body = "My third body";
        await api.Pages.SaveAsync(page3);

        var page4 = await MyCollectionPage.CreateAsync(api);
        page4.SiteId = SITEID;
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
        page5.SiteId = SITEID;
        page5.Title = "Blog Archive";
        await api.Pages.SaveAsync(page5);

        var page6 = await MyDIPage.CreateAsync(api);
        page6.Id = PAGEDIID;
        page6.SiteId = SITEID;
        page6.Title = "My Injection Page";
        await api.Pages.SaveAsync(page6);

        var page7 = await MyPage.CreateAsync(api);
        page7.Id = PAGE7ID;
        page7.SiteId = SITEID;
        page7.Title = "My base page";
        page7.Ingress = "My base ingress";
        page7.Body = "My base body";
        page7.ParentId = PAGE1ID;
        page7.SortOrder = 1;
        await api.Pages.SaveAsync(page7);

        var page8 = await MyPage.CreateAsync(api);
        page8.OriginalPageId = PAGE7ID;
        page8.Id = PAGE8ID;
        page8.SiteId = SITEID;
        page8.Title = "My copied page";
        page8.ParentId = PAGE1ID;
        page8.SortOrder = 2;
        page8.IsHidden = true;
        page8.Route = "test-route";

        await api.Pages.SaveAsync(page8);
    }

    public override async Task DisposeAsync()
    {
        using var api = CreateApi();
        var pages = await api.Pages.GetAllAsync(SITEID);

        foreach (var page in pages.Where(p => !string.IsNullOrEmpty(p.OriginalPageId)))
        {
            await api.Pages.DeleteAsync(page);
        }

        foreach (var page in pages.Where(p => !string.IsNullOrEmpty(p.ParentId)))
        {
            await api.Pages.DeleteAsync(page);
        }

        foreach (var page in pages.Where(p => string.IsNullOrEmpty(p.ParentId)))
        {
            await api.Pages.DeleteAsync(page);
        }

        var types = await api.PageTypes.GetAllAsync();
        foreach (var t in types)
        {
            await api.PageTypes.DeleteAsync(t);
        }

        var site = await api.Sites.GetByIdAsync(SITEID);
        if (site != null)
        {
            await api.Sites.DeleteAsync(site);
        }
    }

    [Fact]
    public void IsCached()
    {
        using var api = CreateApi();
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(PageTestsMemoryCache) ||
            this.GetType() == typeof(PageTestsDistributedCache));
    }

    [Fact]
    public async Task GetNoneById()
    {
        using var api = CreateApi();
        var none = await api.Pages.GetByIdAsync(Snowflake.NewId());

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneBySlug()
    {
        using var api = CreateApi();
        var none = await api.Pages.GetBySlugAsync("none-existing-slug");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetStartpage()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetStartpageAsync();

        Assert.NotNull(model);
        Assert.Null(model.ParentId);
        Assert.Equal(0, model.SortOrder);
    }

    [Fact]
    public async Task GetStartpageBySite()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetStartpageAsync(SITEID);

        Assert.NotNull(model);
        Assert.Null(model.ParentId);
        Assert.Equal(0, model.SortOrder);
    }

    [Fact]
    public async Task GetStartpageNone()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetStartpageAsync(SITEEMPTYID);

        Assert.Null(model);
    }

    [Fact]
    public async Task GetIdBySlug()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetIdBySlugAsync("my-first-page");

        Assert.NotNull(model);
        Assert.Equal(PAGE1ID, model);
    }

    [Fact]
    public async Task GetIdBySlugSiteId()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetIdBySlugAsync("my-first-page", SITEID);

        Assert.NotNull(model);
        Assert.Equal(PAGE1ID, model);
    }

    [Fact]
    public async Task GetAll()
    {
        using var api = CreateApi();
        var pages = await api.Pages.GetAllAsync(SITEID);

        Assert.NotNull(pages);
        Assert.NotEmpty(pages);
    }

    [Fact]
    public async Task GetAllByBaseClass()
    {
        using var api = CreateApi();
        var pages = await api.Pages.GetAllAsync<Models.PageBase>(SITEID);

        Assert.NotNull(pages);
        Assert.NotEmpty(pages);
    }

    [Fact]
    public async Task GetAllBlogs()
    {
        using var api = CreateApi();
        var pages = await api.Pages.GetAllBlogsAsync(SITEID);

        Assert.NotNull(pages);
        Assert.NotEmpty(pages);
    }

    [Fact]
    public async Task GetAllBlogsByBaseClass()
    {
        using var api = CreateApi();
        var pages = await api.Pages.GetAllBlogsAsync<MyBlogPage>(SITEID);

        Assert.NotNull(pages);
        Assert.NotEmpty(pages);
    }

    [Fact]
    public async Task GetAllByMissing()
    {
        using var api = CreateApi();
        var pages = await api.Pages.GetAllAsync<MissingPage>(SITEID);

        Assert.NotNull(pages);
        Assert.Empty(pages);
    }

    [Fact]
    public async Task GetGenericById()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync<MyPage>(PAGE1ID);

        Assert.NotNull(model);
        Assert.Equal("my-first-page", model.Slug);
        Assert.Equal("My first body", model.Body.Value);
    }

    [Fact]
    public async Task GetBaseClassById()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync<Models.PageBase>(PAGE1ID);

        Assert.NotNull(model);
        Assert.Equal(typeof(MyPage), model.GetType());
        Assert.Equal("my-first-page", model.Slug);
        Assert.Equal("Keywords", model.MetaKeywords);
        Assert.Equal("Description", model.MetaDescription);
        Assert.Equal("Og Title", model.OgTitle);
        Assert.Equal("Og Description", model.OgDescription);
        Assert.True(model.MetaFollow);
        Assert.True(model.MetaFollow);

        Assert.Equal("My first body", ((MyPage)model).Body.Value);
    }

    [Fact]
    public async Task GetBlocksById()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync<MyPage>(PAGE1ID);

        Assert.NotNull(model);
        Assert.Equal(2, model.Blocks.Count);
        Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[0]);
        Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[1]);
    }

    [Fact]
    public async Task GetMissingById()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync<MissingPage>(PAGE1ID);

        Assert.Null(model);
    }

    [Fact]
    public async Task GetInfoById()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync<Models.PageInfo>(PAGE1ID);

        Assert.NotNull(model);
        Assert.Equal("my-first-page", model.Slug);
        Assert.Empty(model.Blocks);
    }

    [Fact]
    public async Task GetMultipleBaseClassById()
    {
        using var api = CreateApi();
        var models = await api.Pages.GetByIdsAsync<Models.PageBase>(PAGE1ID, PAGE2ID, PAGE3ID);

        Assert.NotEmpty(models);
        Assert.Equal(3, models.Count());
    }

    [Fact]
    public async Task GetGenericBySlug()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetBySlugAsync<MyPage>("my-first-page");

        Assert.NotNull(model);
        Assert.Equal("my-first-page", model.Slug);
        Assert.Equal("My first body", model.Body.Value);
    }

    [Fact]
    public async Task GetBaseClassBySlug()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetBySlugAsync<Models.PageBase>("my-first-page");

        Assert.NotNull(model);
        Assert.Equal(typeof(MyPage), model.GetType());
        Assert.Equal("my-first-page", model.Slug);
        Assert.Equal("My first body", ((MyPage)model).Body.Value);
    }

    [Fact]
    public async Task GetMissingBySlug()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetBySlugAsync<MissingPage>("my-first-page");

        Assert.Null(model);
    }

    [Fact]
    public async Task GetInfoBySlug()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetBySlugAsync<Models.PageInfo>("my-first-page");

        Assert.NotNull(model);
        Assert.Equal("my-first-page", model.Slug);
        Assert.Empty(model.Blocks);
    }

    [Fact]
    public async Task GetDynamicById()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync(PAGE1ID);

        Assert.NotNull(model);
        Assert.Equal("my-first-page", model.Slug);
        Assert.Equal("My first body", model.Regions.Body.Value);
    }

    [Fact]
    public async Task GetDynamicBySlug()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetBySlugAsync("my-first-page");

        Assert.NotNull(model);
        Assert.Equal("My first page", model.Title);
        Assert.Equal("My first body", model.Regions.Body.Value);
    }

    [Fact]
    public async Task CheckPermlinkSyntax()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync(PAGE1ID);

        Assert.NotNull(model);
        Assert.NotNull(model.Permalink);
        Assert.StartsWith("/", model.Permalink);
    }

    [Fact]
    public async Task GetCollectionPage()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetBySlugAsync<MyCollectionPage>("my-collection-page");

        Assert.NotNull(page);
        Assert.Equal(3, page.Texts.Count);
        Assert.Equal("Second text", page.Texts[1].Value);
    }

    [Fact]
    public async Task GetCollectionPageBaseClass()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetBySlugAsync<Models.PageBase>("my-collection-page");

        Assert.NotNull(page);
        Assert.Equal(typeof(MyCollectionPage), page.GetType());
        Assert.Equal(3, ((MyCollectionPage)page).Texts.Count);
        Assert.Equal("Second text", ((MyCollectionPage)page).Texts[1].Value);
    }

    [Fact]
    public async Task GetDynamicCollectionPage()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetBySlugAsync("my-collection-page");

        Assert.NotNull(page);
        Assert.Equal(3, page.Regions.Texts.Count);
        Assert.Equal("Second text", page.Regions.Texts[1].Value);
    }

    [Fact]
    public async Task EmptyCollectionPage()
    {
        using var api = CreateApi();
        var page = await MyCollectionPage.CreateAsync(api);

        Assert.Empty(page.Texts);

        page.SiteId = SITEID;
        page.Title = "Another collection page";

        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetBySlugAsync<MyCollectionPage>(Aero.Cms.Utils.GenerateSlug(page.Title), SITEID);

        Assert.Empty(page.Texts);
    }

    [Fact]
    public async Task EmptyDynamicCollectionPage()
    {
        using var api = CreateApi();
        var page = await Aero.Cms.Models.DynamicPage.CreateAsync(api, "MyCollectionPage");

        Assert.Equal(0, page.Regions.Texts.Count);

        page.SiteId = SITEID;
        page.Title = "Third collection page";

        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetBySlugAsync(Aero.Cms.Utils.GenerateSlug(page.Title), SITEID);

        Assert.Equal(0, page.Regions.Texts.Count);
    }

    [Fact]
    public async Task EmptyCollectionPageComplex()
    {
        using var api = CreateApi();
        var page = await MyCollectionPage.CreateAsync(api);

        Assert.Empty(page.Teasers);

        page.SiteId = SITEID;
        page.Title = "Fourth collection page";

        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetBySlugAsync<MyCollectionPage>(Aero.Cms.Utils.GenerateSlug(page.Title), SITEID);

        Assert.Empty(page.Teasers);
    }

    [Fact]
    public async Task EmptyDynamicCollectionPageComplex()
    {
        using var api = CreateApi();
        var page = await Aero.Cms.Models.DynamicPage.CreateAsync(api, "MyCollectionPage");

        Assert.Equal(0, page.Regions.Teasers.Count);

        page.SiteId = SITEID;
        page.Title = "Fifth collection page";

        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetBySlugAsync(Aero.Cms.Utils.GenerateSlug(page.Title), SITEID);

        Assert.Equal(0, page.Regions.Teasers.Count);
    }

    [Fact]
    public async Task Add()
    {
        var count = (await api.Pages.GetAllAsync(SITEID)).Count();
        var page = await MyPage.CreateAsync(api, "MyPage");
        page.SiteId = SITEID;
        page.Title = "My fourth page";
        page.Ingress = "My fourth ingress";
        page.Body = "My fourth body";

        await api.Pages.SaveDraftAsync(page);

        Assert.Equal(count + 1, (await api.Pages.GetAllAsync(SITEID)).Count());
    }

    [Fact]
    public async Task AddHierarchical()
    {
        using var api = CreateApi();
        var page = await MyPage.CreateAsync(api, "MyPage");
        page.Id = Snowflake.NewId();
        page.ParentId = PAGE1ID;
        page.SiteId = SITEID;
        page.Title = "My subpage";
        page.Ingress = "My subpage ingress";
        page.Body = "My subpage body";

        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetByIdAsync<MyPage>(page.Id);

        Assert.NotNull(page);
        Assert.Equal("my-first-page/my-subpage", page.Slug);
    }

    [Fact]
    public async Task AddNonHierarchical()
    {
        using var api = CreateApi();
        using (var config = new Aero.Cms.Config(api))
        {
            config.HierarchicalPageSlugs = false;
        }

        var page = await MyPage.CreateAsync(api, "MyPage");
        page.Id = Snowflake.NewId();
        page.ParentId = PAGE1ID;
        page.SiteId = SITEID;
        page.Title = "My second subpage";
        page.Ingress = "My subpage ingress";
        page.Body = "My subpage body";

        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetByIdAsync<MyPage>(page.Id);

        Assert.NotNull(page);
        Assert.Equal("my-second-subpage", page.Slug);

        using (var config = new Aero.Cms.Config(api))
        {
            config.HierarchicalPageSlugs = true;
        }
    }

    [Fact]
    public async Task AddDuplicateSlugShouldThrow()
    {
        using var api = CreateApi();
        var page = await MyPage.CreateAsync(api);
        page.SiteId = SITEID;
        page.Title = "My first page";
        page.Published = DateTime.Now;

        await Assert.ThrowsAsync<ValidationException>(async () => { await api.Pages.SaveAsync(page); });
    }

    [Fact]
    public async Task Update()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetByIdAsync<MyPage>(PAGE1ID);

        Assert.NotNull(page);
        Assert.Equal("My first page", page.Title);

        page.Title = "Updated page";
        page.IsHidden = true;
        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetByIdAsync<MyPage>(PAGE1ID);

        Assert.NotNull(page);
        Assert.Equal("Updated page", page.Title);
        Assert.True(page.IsHidden);
    }

    [Fact]
    public async Task SaveDraft()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetByIdAsync<MyPage>(PAGE1ID);

        Assert.NotNull(page);

        page.Title = "My working copy";
        await api.Pages.SaveDraftAsync(page);

        page = await api.Pages.GetByIdAsync<MyPage>(PAGE1ID);

        Assert.NotNull(page);
        Assert.NotEqual("My working copy", page.Title);

        page = await api.Pages.GetDraftByIdAsync<MyPage>(PAGE1ID);

        Assert.NotNull(page);
        Assert.Equal("My working copy", page.Title);
    }

    [Fact]
    public async Task UpdateCollectionPage()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetBySlugAsync<MyCollectionPage>("my-collection-page", SITEID);

        Assert.NotNull(page);
        Assert.Equal(3, page.Texts.Count);
        Assert.Equal("First text", page.Texts[0].Value);

        page.Texts[0] = "Updated text";
        page.Texts.RemoveAt(2);
        await api.Pages.SaveAsync(page);

        page = await api.Pages.GetBySlugAsync<MyCollectionPage>("my-collection-page", SITEID);

        Assert.NotNull(page);
        Assert.Equal(2, page.Texts.Count);
        Assert.Equal("Updated text", page.Texts[0].Value);
    }

    [Fact]
    public async Task Move()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetByIdAsync(PAGE1ID);

        Assert.NotNull(page);
        Assert.True(page.SortOrder > 0);

        page.SortOrder = 0;
        await api.Pages.MoveAsync(page, null, 0);

        page = await api.Pages.GetByIdAsync(PAGE1ID);

        Assert.NotNull(page);
        Assert.Equal(0, page.SortOrder);
    }

    [Fact]
    public async Task Delete()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetByIdAsync<MyPage>(PAGE3ID);
        var count = (await api.Pages.GetAllAsync(SITEID)).Count();

        Assert.NotNull(page);

        await api.Pages.DeleteAsync(page);

        Assert.Equal(count - 1, (await api.Pages.GetAllAsync(SITEID)).Count());
    }

    [Fact]
    public async Task DeleteById()
    {
        using var api = CreateApi();
        var count = (await api.Pages.GetAllAsync(SITEID)).Count();

        await api.Pages.DeleteAsync(PAGE2ID);

        Assert.Equal(count - 1, (await api.Pages.GetAllAsync(SITEID)).Count());
    }

    [Fact]
    public async Task GetDIGeneric()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetByIdAsync<MyDIPage>(PAGEDIID);

        Assert.NotNull(page);
        Assert.Equal("My service value", page.Body.Value);
    }

    [Fact]
    public async Task GetDIDynamic()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetByIdAsync(PAGEDIID);

        Assert.NotNull(page);
        Assert.Equal("My service value", page.Regions.Body.Value);
    }

    [Fact]
    public async Task CreateDIGeneric()
    {
        using var api = CreateApi();
        var page = await MyDIPage.CreateAsync(api);

        Assert.NotNull(page);
        Assert.Equal("My service value", page.Body.Value);
    }

    [Fact]
    public async Task CreateDIDynamic()
    {
        using var api = CreateApi();
        var page = await Models.DynamicPage.CreateAsync(api, nameof(MyDIPage));

        Assert.NotNull(page);
        Assert.Equal("My service value", page.Regions.Body.Value);
    }

    [Fact]
    public async Task GetCopyGenericById()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetByIdAsync<MyPage>(PAGE8ID);

        Assert.NotNull(model);
        Assert.Equal("My copied page", model.Title);
        Assert.Equal("my-first-page/my-copied-page", model.Slug);
        Assert.Equal(PAGE1ID, model.ParentId);
        Assert.Equal(2, model.SortOrder);
        Assert.True(model.IsHidden);
        Assert.Equal("test-route", model.Route);

        Assert.Equal(PAGE7ID, model.OriginalPageId);
        Assert.Equal("My base body", model.Body.Value);
    }

    [Fact]
    public async Task GetCopyGenericBySlug()
    {
        using var api = CreateApi();
        var model = await api.Pages.GetBySlugAsync<MyPage>("my-first-page/my-copied-page");

        Assert.NotNull(model);
        Assert.Equal("My copied page", model.Title);
        Assert.Equal("my-first-page/my-copied-page", model.Slug);
        Assert.Equal(PAGE1ID, model.ParentId);
        Assert.Equal(2, model.SortOrder);
        Assert.True(model.IsHidden);
        Assert.Equal("test-route", model.Route);

        Assert.Equal(PAGE7ID, model.OriginalPageId);
        Assert.Equal("My base body", model.Body.Value);
    }

    [Fact]
    public async Task UpdatingCopyShouldIgnoreBodyAndDate()
    {
        using var api = CreateApi();
        var page = await api.Pages.GetByIdAsync<MyPage>(PAGE8ID);
        page.Created = DateTime.Parse("2001-01-01");
        page.LastModified = DateTime.Parse("2001-01-01");
        page.Body = "My edits to the body";

        await api.Pages.SaveAsync(page);
        page = await api.Pages.GetByIdAsync<MyPage>(PAGE8ID);

        Assert.NotEqual(DateTime.Parse("2001-01-01"), page.Created);
        Assert.NotEqual(DateTime.Parse("2001-01-01"), page.LastModified);
        Assert.NotEqual("My edits to the body", page.Body.ToString());
    }

    [Fact]
    public async Task CanNotUpdateCopyOriginalPageWithAnotherCopy()
    {
        using var api = CreateApi();
        var page = await MyPage.CreateAsync(api);
        page.Title = "New title";
        page.OriginalPageId = PAGE8ID; // PAGE8 is an copy of PAGE7

        var exn = await Assert.ThrowsAsync<InvalidOperationException>(async () => { await api.Pages.SaveAsync(page); });

        Assert.Equal("Can not set copy of a copy", exn.Message);
    }

    [Fact]
    public async Task CanNotUpdateCopyWithAnotherTypeIdOtherThanOriginalPageTypeId()
    {
        using var api = CreateApi();
        var page = await MissingPage.CreateAsync(api);
        page.Title = "New title";
        page.OriginalPageId = PAGE7ID;

        var exn = await Assert.ThrowsAsync<InvalidOperationException>(async () => { await api.Pages.SaveAsync(page); });

        Assert.Equal("Copy can not have a different content type", exn.Message);
    }

    [Fact]
    public async Task DetachShouldCopyBlocks()
    {
        using var api = CreateApi();
        var originalPage = await api.Pages.GetByIdAsync<MyPage>(PAGE7ID);
        var copy = await api.Pages.GetByIdAsync<MyPage>(PAGE8ID);
        var originalBlock = new Extend.Blocks.TextBlock
        {
            Id = Snowflake.NewId(),
            Body = "test",
        };

        originalPage.Blocks.Add(originalBlock);
        await api.Pages.SaveAsync(originalPage);

        await api.Pages.DetachAsync(copy);

        var p = await api.Pages.GetByIdAsync<MyPage>(PAGE8ID);
        Assert.Collection(p.Blocks, e =>
        {
            Assert.NotEqual(e.Id, originalBlock.Id);
            var eBlock = Assert.IsType<Extend.Blocks.TextBlock>(e);
            Assert.Equal(eBlock.Body.Value, originalBlock.Body.Value);
        });
    }

    [Fact]
    public async Task DetachShouldCopyRegions()
    {
        using var api = CreateApi();
        var originalPage = await api.Pages.GetByIdAsync<MyPage>(PAGE7ID);
        originalPage.Body = "body to be copied";
        originalPage.Ingress = "ingress to be copied";
        await api.Pages.SaveAsync(originalPage);

        var copy = await api.Pages.GetByIdAsync<MyPage>(PAGE8ID);
        await api.Pages.DetachAsync(copy);

        originalPage = await api.Pages.GetByIdAsync<MyPage>(PAGE7ID);
        originalPage.Body = "body should not be copied";
        originalPage.Ingress = "ingress should not be copied";
        await api.Pages.SaveAsync(originalPage);

        var p = await api.Pages.GetByIdAsync<MyPage>(PAGE8ID);
        // Wait, IDb is not public or accessible easily. Let's just serialize `p` to throw it in the error message.
        if (p.Body == null)
        {
            throw new Exception("p.Body is null! Serialized p: " + System.Text.Json.JsonSerializer.Serialize(p));
        }

        Assert.NotNull(p);
        Assert.NotNull(p.Body);
        Assert.Equal("body to be copied", p.Body.Value);
        Assert.Equal("ingress to be copied", p.Ingress.Value);
    }

    [Fact]
    public async Task DeleteCascadeDeletesCopies()
    {
        // After RavenDB/NoSQL refactor: deleting a page with copies now 
        // cascade deletes the copies first, so deletion succeeds.
        using var api = CreateApi();
        var count = (await api.Pages.GetAllAsync(SITEID)).Count();

        // This should now succeed - copies are deleted first
        await api.Pages.DeleteAsync(PAGE7ID);

        // Both the copy (PAGE8) and original (PAGE7) should be deleted
        Assert.Equal(count - 2, (await api.Pages.GetAllAsync(SITEID)).Count());
    }
}
