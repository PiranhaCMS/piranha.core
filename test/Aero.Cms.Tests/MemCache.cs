

using Microsoft.Extensions.Caching.Memory;

namespace Aero.Cms.Tests;

public class MemCache(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private readonly string id1 = Snowflake.NewId().ToString();
    private readonly string id2 = Snowflake.NewId().ToString();
    private readonly string id3 = Snowflake.NewId().ToString();
    private readonly string val1 = "My first value";
    private readonly string val2 = "My second value";
    private readonly string val3 = "My third value";
    private readonly string val4 = "My fourth value";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));

        await cache.SetAsync(id1, val1);
        await cache.SetAsync(id2, val2);
    }

    public override Task DisposeAsync()
    {
        return Task.Run(() => { });
    }

    [Fact]
    public async Task AddEntry()
    {
        await cache.SetAsync(id3, val3);
    }

    [Fact]
    public async Task GetEntry()
    {
        var val = await cache.GetAsync<string>(id2);

        Assert.NotNull(val);
        Assert.Equal(val2, val);
    }

    [Fact]
    public async Task UpdateEntry()
    {
        await cache.SetAsync(id2, val4);

        var val = await cache.GetAsync<string>(id2);

        Assert.NotNull(val);
        Assert.Equal(val4, val);
    }

    [Fact]
    public async Task RemoveEntry()
    {
        await cache.RemoveAsync(id1);

        var val = await cache.GetAsync<string>(id1);

        Assert.Null(val);
    }
}
