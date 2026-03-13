

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

//[Collection("Integration tests")]
public class PageTypeTestsMemoryCache(MartenFixture fixture) : PageTypeTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

//[Collection("Integration tests")]
public class PageTypeTestsDistributedCache(MartenFixture fixture) : PageTypeTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

//[Collection("Integration tests")]
public class PageTypeTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private readonly List<PageType> pageTypes = new List<PageType>
    {
        new PageType
        {
            Id = "MyFirstType",
            Regions = new List<ContentTypeRegion>
            {
                new ContentTypeRegion
                {
                    Id = "Body",
                    Fields = new List<ContentTypeField>
                    {
                        new ContentTypeField
                        {
                            Id = "Default",
                            Type = "Html"
                        }
                    }
                }
            }
        },
        new PageType
        {
            Id = "MySecondType",
            Regions = new List<ContentTypeRegion>
            {
                new ContentTypeRegion
                {
                    Id = "Body",
                    Fields = new List<ContentTypeField>
                    {
                        new ContentTypeField
                        {
                            Id = "Default",
                            Type = "Text"
                        }
                    }
                }
            }
        },
        new PageType
        {
            Id = "MyThirdType",
            Regions = new List<ContentTypeRegion>
            {
                new ContentTypeRegion
                {
                    Id = "Body",
                    Fields = new List<ContentTypeField>
                    {
                        new ContentTypeField
                        {
                            Id = "Default",
                            Type = "Image"
                        }
                    }
                }
            }
        },
        new PageType
        {
            Id = "MyFourthType",
            Regions = new List<ContentTypeRegion>
            {
                new ContentTypeRegion
                {
                    Id = "Body",
                    Fields = new List<ContentTypeField>
                    {
                        new ContentTypeField
                        {
                            Id = "Default",
                            Type = "String"
                        }
                    }
                }
            }
        },
        new PageType
        {
            Id = "MyFifthType",
            Regions = new List<ContentTypeRegion>
            {
                new ContentTypeRegion
                {
                    Id = "Body",
                    Fields = new List<ContentTypeField>
                    {
                        new ContentTypeField
                        {
                            Id = "Default",
                            Type = "Text"
                        }
                    }
                }
            }
        }
    };

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        
        await api.PageTypes.SaveAsync(pageTypes[0]);
        await api.PageTypes.SaveAsync(pageTypes[3]);
        await api.PageTypes.SaveAsync(pageTypes[4]);
    }

    public override async Task DisposeAsync()
    {
        
        var pageTypes = await api.PageTypes.GetAllAsync();

        foreach (var p in pageTypes)
        {
            await api.PageTypes.DeleteAsync(p);
        }
    }

    [Fact]
    public void IsCached()
    {
        
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(PageTypeTestsMemoryCache) ||
            this.GetType() == typeof(PageTypeTestsDistributedCache));
    }

    [Fact]
    public async Task Add()
    {
        
        await api.PageTypes.SaveAsync(pageTypes[1]);
    }

    [Fact]
    public async Task GetAll()
    {
        
        var models = await api.PageTypes.GetAllAsync();

        Assert.NotNull(models);
        Assert.NotEmpty(models);
    }

    [Fact]
    public async Task GetNoneById()
    {
        
        var none = await api.PageTypes.GetByIdAsync("none-existing-type");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetById()
    {
        
        var model = await api.PageTypes.GetByIdAsync(pageTypes[0].Id);

        Assert.NotNull(model);
        Assert.Equal(pageTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
    }

    [Fact]
    public async Task Update()
    {
        
        var model = await api.PageTypes.GetByIdAsync(pageTypes[0].Id);

        Assert.Null(model.Title);

        model.Title = "Updated";

        await api.PageTypes.SaveAsync(model);
    }

    [Fact]
    public async Task Delete()
    {
        
        var model = await api.PageTypes.GetByIdAsync(pageTypes[3].Id);

        Assert.NotNull(model);

        await api.PageTypes.DeleteAsync(model);
    }

    [Fact]
    public async Task DeleteById()
    {
        
        var model = await api.PageTypes.GetByIdAsync(pageTypes[4].Id);

        Assert.NotNull(model);

        await api.PageTypes.DeleteAsync(model.Id);
    }
}
