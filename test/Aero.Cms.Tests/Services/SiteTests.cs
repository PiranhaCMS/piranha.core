

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Aero.Cms.AttributeBuilder;
using Aero.Cms.Extend;
using Aero.Cms.Extend.Fields;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

//[Collection("Integration tests")]
public class SiteTestsMemoryCache(MartenFixture fixture) : SiteTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

//[Collection("Integration tests")]
public class SiteTestsDistributedCache(MartenFixture fixture) : SiteTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

//[Collection("Integration tests")]
public class SiteTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private const string SITE1 = "MyFirstSite";
    private const string SITE2 = "MySecondSite";
    private const string SITE4 = "MyFourthSite";
    private const string SITE5 = "MyFifthSite";
    private const string SITE6 = "MySixthSite";
    private const string SITE1HOSTS = "mysite.com";

    private readonly string SITE1ID = Snowflake.NewId();

    [PageType(Title = "PageType")]
    public class MyPage : Models.Page<MyPage>
    {
        [Region] public TextField Text { get; set; }
    }

    [SiteType(Title = "SiteType")]
    public class MySiteContent : Models.SiteContent<MySiteContent>
    {
        [Region] public HtmlField Header { get; set; }

        [Region] public HtmlField Footer { get; set; }
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        
        Aero.Cms.App.Init(api);

        new ContentTypeBuilder(api)
            .AddType(typeof(MyPage))
            .AddType(typeof(MySiteContent))
            .Build();

        var sites = await api.Sites.GetAllAsync();

        await api.Sites.SaveAsync(new Site
        {
            Id = SITE1ID,
            SiteTypeId = "MySiteContent",
            InternalId = SITE1,
            Title = SITE1,
            Hostnames = SITE1HOSTS,
            IsDefault = true
        });

        await api.Sites.SaveAsync(new Site
        {
            InternalId = SITE4,
            Title = SITE4
        });
        await api.Sites.SaveAsync(new Site
        {
            InternalId = SITE5,
            Title = SITE5
        });
        await api.Sites.SaveAsync(new Site
        {
            InternalId = SITE6,
            Title = SITE6
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
        await api.Sites.SaveContentAsync(SITE1ID, content);

        var page1 = await MyPage.CreateAsync(api);
        page1.SiteId = SITE1ID;
        page1.Title = "Startpage";
        page1.Text = "Welcome";
        page1.IsHidden = true;
        page1.Published = DateTime.Now;
        await api.Pages.SaveAsync(page1);

        var page2 = await MyPage.CreateAsync(api);
        page2.SiteId = SITE1ID;
        page2.SortOrder = 1;
        page2.Title = "Second page";
        page2.Text = "The second page";
        await api.Pages.SaveAsync(page2);

        var page3 = await MyPage.CreateAsync(api);
        page3.SiteId = SITE1ID;
        page3.ParentId = page2.Id;
        page3.Title = "Subpage";
        page3.Text = "The subpage";
        page3.Published = DateTime.Now;
        await api.Pages.SaveAsync(page3);
    }

    public override async Task DisposeAsync()
    {
        
        var pages = await api.Pages.GetAllAsync(SITE1ID);
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

    [Fact]
    public void IsCached()
    {
        
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(SiteTestsMemoryCache) ||
            this.GetType() == typeof(SiteTestsDistributedCache));
    }

    [Fact]
    public async Task Add()
    {
        
        await api.Sites.SaveAsync(new Site
        {
            InternalId = SITE2,
            Title = SITE2
        });
    }

    [Fact]
    public async Task AddDuplicateKey()
    {
        
        await Assert.ThrowsAnyAsync<ValidationException>(async () =>
            await api.Sites.SaveAsync(new Site
            {
                InternalId = SITE1,
                Title = SITE1
            }));
    }

    [Fact]
    public async Task AddEmptyFailure()
    {
        
        await Assert.ThrowsAnyAsync<ValidationException>(async () =>
            await api.Sites.SaveAsync(new Site()));
    }

    [Fact]
    public async Task AddAndGenerateInternalId()
    {
        var id = Snowflake.NewId();

        
        await api.Sites.SaveAsync(new Site
        {
            Id = id,
            Title = "Generate internal id"
        });

        var site = await api.Sites.GetByIdAsync(id);

        Assert.NotNull(site);
        Assert.Equal("GenerateInternalId", site.InternalId);
    }

    [Fact]
    public async Task GetAll()
    {
        
        var models = await api.Sites.GetAllAsync();

        Assert.NotNull(models);
        Assert.NotEmpty(models);
    }

    [Fact]
    public async Task GetNoneById()
    {
        
        var none = await api.Sites.GetByIdAsync(Snowflake.NewId());

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneByInternalId()
    {
        
        var none = await api.Sites.GetByInternalIdAsync("none-existing-id");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetById()
    {
        
        var model = await api.Sites.GetByIdAsync(SITE1ID);

        Assert.NotNull(model);
        Assert.Equal(SITE1, model.InternalId);
    }

    [Fact]
    public async Task GetByInternalId()
    {
        
        var model = await api.Sites.GetByInternalIdAsync(SITE1);

        Assert.NotNull(model);
        Assert.Equal(SITE1, model.InternalId);
    }

    [Fact]
    public async Task GetDefault()
    {
        
        var model = await api.Sites.GetDefaultAsync();

        Assert.NotNull(model);
        Assert.Equal(SITE1, model.InternalId);
    }

    [Fact]
    public async Task GetSitemap()
    {
        
        var sitemap = await api.Sites.GetSitemapAsync();

        Assert.NotNull(sitemap);
        Assert.NotEmpty(sitemap);
        Assert.Equal("Startpage", sitemap[0].Title);
    }

    [Fact]
    public async Task GetSitemapById()
    {
        
        var sitemap = await api.Sites.GetSitemapAsync(SITE1ID);

        Assert.NotNull(sitemap);
        Assert.NotEmpty(sitemap);
        Assert.Equal("Startpage", sitemap[0].Title);
    }

    [Fact]
    public async Task CheckPermlinkSyntax()
    {
        
        var sitemap = await api.Sites.GetSitemapAsync();

        foreach (var item in sitemap)
        {
            Assert.NotNull(item.Permalink);
            Assert.StartsWith("/", item.Permalink);
        }
    }

    [Fact]
    public async Task GetUnpublishedSitemap()
    {
        
        var sitemap = await api.Sites.GetSitemapAsync(onlyPublished: false);

        Assert.NotNull(sitemap);
        Assert.Equal(2, sitemap.Count);
        Assert.Equal("Startpage", sitemap[0].Title);
        Assert.Single(sitemap[1].Items);
        Assert.Equal("Subpage", sitemap[1].Items[0].Title);
    }

    [Fact]
    public async Task CheckHiddenSitemapItems()
    {
        
        var sitemap = await api.Sites.GetSitemapAsync();

        Assert.Equal(1, sitemap.Count(s => s.IsHidden));
    }

    [Fact]
    public async Task ChangeDefaultSite()
    {
        
        var site6 = await api.Sites.GetByInternalIdAsync(SITE6);

        Assert.False(site6.IsDefault);
        site6.IsDefault = true;
        await api.Sites.SaveAsync(site6);

        var site1 = await api.Sites.GetByIdAsync(SITE1ID);

        Assert.False(site1.IsDefault);
        site1.IsDefault = true;
        await api.Sites.SaveAsync(site1);
    }

    [Fact]
    public async Task CantRemoveDefault()
    {
        
        var site1 = await api.Sites.GetByIdAsync(SITE1ID);

        Assert.True(site1.IsDefault);
        site1.IsDefault = false;
        await api.Sites.SaveAsync(site1);

        site1 = await api.Sites.GetByIdAsync(SITE1ID);

        Assert.True(site1.IsDefault);
    }

    [Fact]
    public async Task GetUnpublishedSitemapById()
    {
        
        var sitemap = await api.Sites.GetSitemapAsync(SITE1ID, onlyPublished: false);

        Assert.NotNull(sitemap);
        Assert.Equal(2, sitemap.Count);
        Assert.Equal("Startpage", sitemap[0].Title);
        Assert.Single(sitemap[1].Items);
        Assert.Equal("Subpage", sitemap[1].Items[0].Title);
    }

    [Fact]
    public async Task Update()
    {
        
        var model = await api.Sites.GetByIdAsync(SITE1ID);

        Assert.Equal(SITE1HOSTS, model.Hostnames);

        model.Hostnames = "Updated";

        await api.Sites.SaveAsync(model);
    }

    [Fact]
    public async Task Delete()
    {
        
        var model = await api.Sites.GetByInternalIdAsync(SITE4);

        Assert.NotNull(model);

        await api.Sites.DeleteAsync(model);
    }

    [Fact]
    public async Task DeleteById()
    {
        
        var model = await api.Sites.GetByInternalIdAsync(SITE5);

        Assert.NotNull(model);

        await api.Sites.DeleteAsync(model.Id);
    }

    [Fact]
    public async Task GetSiteContent()
    {
        
        var model = await api.Sites.GetContentByIdAsync<MySiteContent>(SITE1ID);

        Assert.NotNull(model);
        Assert.Equal("<p>Lorem ipsum</p>", model.Header.Value);
    }

    [Fact]
    public async Task UpdateSiteContent()
    {
        
        var model = await api.Sites.GetContentByIdAsync<MySiteContent>(SITE1ID);

        Assert.NotNull(model);
        model.Footer = "<p>Fusce Parturient</p>";
        await api.Sites.SaveContentAsync(SITE1ID, model);

        model = await api.Sites.GetContentByIdAsync<MySiteContent>(SITE1ID);
        Assert.NotNull(model);
        Assert.Equal("<p>Fusce Parturient</p>", model.Footer.Value);
    }

    [Fact]
    public async Task GetDynamicSiteContent()
    {
        
        var model = await api.Sites.GetContentByIdAsync(SITE1ID);

        Assert.NotNull(model);
        Assert.Equal("<p>Lorem ipsum</p>", model.Regions.Header.Value);
    }

    [Fact]
    public async Task UpdateDynamicSiteContent()
    {
        
        var model = await api.Sites.GetContentByIdAsync(SITE1ID);

        Assert.NotNull(model);
        model.Regions.Footer.Value = "<p>Purus Sit</p>";
        await api.Sites.SaveContentAsync(SITE1ID, model);

        model = await api.Sites.GetContentByIdAsync(SITE1ID);
        Assert.NotNull(model);
        Assert.Equal("<p>Purus Sit</p>", model.Regions.Footer.Value);
    }

    [Fact]
    public async Task GetByHostname()
    {
        
        var model = await api.Sites.GetByHostnameAsync("mydomain.com");

        Assert.NotNull(model);
        Assert.Equal("RoutingTest1", model.InternalId);
    }

    [Fact]
    public async Task GetByHostnameSecond()
    {
        
        var model = await api.Sites.GetByHostnameAsync("localhost");

        Assert.NotNull(model);
        Assert.Equal("RoutingTest1", model.InternalId);
    }

    [Fact]
    public async Task GetByHostnameSuffix()
    {
        
        var model = await api.Sites.GetByHostnameAsync("mydomain.com/en");

        Assert.NotNull(model);
        Assert.Equal("RoutingTest2", model.InternalId);
    }

    [Fact]
    public async Task GetByHostnameSubdomain()
    {
        
        var model = await api.Sites.GetByHostnameAsync("sub.mydomain.com");

        Assert.NotNull(model);
        Assert.Equal("RoutingTest3", model.InternalId);
    }

    [Fact]
    public async Task GetByHostnameSubdomainSecond()
    {
        
        var model = await api.Sites.GetByHostnameAsync("sub2.localhost");

        Assert.NotNull(model);
        Assert.Equal("RoutingTest3", model.InternalId);
    }

    [Fact]
    public async Task GetByHostnameMissing()
    {
        
        var model = await api.Sites.GetByHostnameAsync("nosite.com");

        Assert.Null(model);
    }
}
