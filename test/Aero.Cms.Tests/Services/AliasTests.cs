

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

//[Collection("Integration tests")]
public class AliasTestsMemoryCache(MartenFixture fixture) : AliasTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

//[Collection("Integration tests")]
public class AliasTestsDistributedCache(MartenFixture fixture) : AliasTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

//[Collection("Integration tests")]
public class AliasTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private const string ALIAS1 = "/old-url";
    private const string ALIAS2 = "/another-old-url";
    private const string ALIAS3 = "/moved/page";
    private const string ALIAS4 = "/another-moved-page";
    private const string ALIAS5 = "/the-last-moved-page";

    private readonly string SITEID = Snowflake.NewId();
    private readonly string ALIAS1ID = Snowflake.NewId();

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        
        // Add site
        var site = new Site
        {
            Id = SITEID,
            Title = "Alias Site",
            InternalId = "AliasSite",
            IsDefault = true
        };
        await api.Sites.SaveAsync(site);

        // Add aliases
        await api.Aliases.SaveAsync(new Alias
        {
            Id = ALIAS1ID,
            SiteId = SITEID,
            AliasUrl = ALIAS1,
            RedirectUrl = "/redirect-1"
        });
        //WaitForUserToContinueTheTest(store);
        await api.Aliases.SaveAsync(new Alias
        {
            SiteId = SITEID,
            AliasUrl = ALIAS4,
            RedirectUrl = "/redirect-4"
        });
        await api.Aliases.SaveAsync(new Alias
        {
            SiteId = SITEID,
            AliasUrl = ALIAS5,
            RedirectUrl = "/redirect-5"
        });
    }

    public override async Task DisposeAsync()
    {
        
        var aliases = await api.Aliases.GetAllAsync();
        foreach (var a in aliases)
        {
            await api.Aliases.DeleteAsync(a);
        }

        var sites = await api.Sites.GetAllAsync();
        foreach (var s in sites)
        {
            await api.Sites.DeleteAsync(s);
        }
    }

    [Fact]
    public void IsCached()
    {
        
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(AliasTestsMemoryCache) ||
            this.GetType() == typeof(AliasTestsDistributedCache));
    }

    [Fact]
    public async Task Add()
    {
        
        await api.Aliases.SaveAsync(new Alias
        {
            SiteId = SITEID,
            AliasUrl = ALIAS2,
            RedirectUrl = "/redirect-2"
        });
    }

    [Fact]
    public async Task AddDuplicateKey()
    {
        
        await Assert.ThrowsAnyAsync<Exception>(async () =>
            await api.Aliases.SaveAsync(new Alias
            {
                SiteId = SITEID,
                AliasUrl = ALIAS1,
                RedirectUrl = "/duplicate-alias"
            })
        );
    }

    [Fact]
    public async Task GetNoneById()
    {
        
        var none = await api.Aliases.GetByIdAsync(Snowflake.NewId());

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneByAliasUrl()
    {
        
        var none = await api.Aliases.GetByAliasUrlAsync("/none-existing-alias");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneByRedirectUrl()
    {
        
        var none = await api.Aliases.GetByRedirectUrlAsync("/none-existing-alias");

        Assert.Empty(none);
    }

    [Fact]
    public async Task GetAll()
    {
        
        var models = await api.Aliases.GetAllAsync();

        Assert.NotNull(models);
        Assert.NotEmpty(models);
    }

    [Fact]
    public async Task GetById()
    {
        
        var model = await api.Aliases.GetByIdAsync(ALIAS1ID);

        Assert.NotNull(model);
        Assert.Equal(ALIAS1, model.AliasUrl);
    }

    [Fact]
    public async Task GetByAliasUrl()
    {
        
        var model = await api.Aliases.GetByAliasUrlAsync(ALIAS1);

        Assert.NotNull(model);
        Assert.Equal(ALIAS1, model.AliasUrl);
    }

    [Fact]
    public async Task GetByAliasUrlWithDifferentCase()
    {
        
        var model = await api.Aliases.GetByAliasUrlAsync("/Old-URL");

        Assert.NotNull(model);
        Assert.Equal(ALIAS1, model.AliasUrl);
    }

    [Fact]
    public async Task GetByRedirectUrl()
    {
        
        var models = await api.Aliases.GetByRedirectUrlAsync("/redirect-1");

        Assert.Single(models);
        Assert.Equal(ALIAS1, models.First().AliasUrl);
    }

    [Fact]
    public async Task GetByRedirectUrlWithDifferentCase()
    {
        
        var models = await api.Aliases.GetByRedirectUrlAsync("/ReDiRect-1");

        Assert.Single(models);
        Assert.Equal(ALIAS1, models.First().AliasUrl);
    }

    [Fact]
    public async Task Update()
    {
        
        var model = await api.Aliases.GetByIdAsync(ALIAS1ID);

        Assert.Equal("/redirect-1", model.RedirectUrl);

        model.RedirectUrl = "/redirect-updated";

        await api.Aliases.SaveAsync(model);
    }

    [Fact]
    public async Task FixAliasUrl()
    {
        
        var model = new Alias
        {
            SiteId = SITEID,
            AliasUrl = "the-alias-url-1",
            RedirectUrl = "/the-redirect-1"
        };

        await api.Aliases.SaveAsync(model);

        Assert.Equal("/the-alias-url-1", model.AliasUrl);
    }

    [Fact]
    public async Task FixRedirectUrl()
    {
        
        var model = new Alias
        {
            SiteId = SITEID,
            AliasUrl = "/the-alias-url-2",
            RedirectUrl = "the-redirect-2"
        };

        await api.Aliases.SaveAsync(model);

        Assert.Equal("/the-redirect-2", model.RedirectUrl);
    }

    [Fact]
    public async Task AllowHttpUrl()
    {
        
        var model = new Alias
        {
            SiteId = SITEID,
            AliasUrl = "/the-alias-url-3",
            RedirectUrl = "http://redirect.com"
        };

        await api.Aliases.SaveAsync(model);

        Assert.Equal("http://redirect.com", model.RedirectUrl);
    }

    [Fact]
    public async Task AllowHttpsUrl()
    {
        
        var model = new Alias
        {
            SiteId = SITEID,
            AliasUrl = "/the-alias-url-4",
            RedirectUrl = "https://redirect.com"
        };

        await api.Aliases.SaveAsync(model);

        Assert.Equal("https://redirect.com", model.RedirectUrl);
    }

    [Fact]
    public async Task Delete()
    {
        
        var model = await api.Aliases.GetByAliasUrlAsync(ALIAS4);

        Assert.NotNull(model);

        model = await api.Aliases.GetByAliasUrlAsync(ALIAS4);

        await api.Aliases.DeleteAsync(model);
    }

    [Fact]
    public async Task DeleteById()
    {
        
        var model = await api.Aliases.GetByAliasUrlAsync(ALIAS5);

        Assert.NotNull(model);

        await api.Aliases.DeleteAsync(model.Id);
    }
}
