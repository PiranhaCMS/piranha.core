

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

//[Collection("Integration tests")]
public class SiteTypeTestsMemoryCache(MartenFixture fixture) : SiteTypeTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

//[Collection("Integration tests")]
public class SiteTypeTestsDistributedCache(MartenFixture fixture) : SiteTypeTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

//[Collection("Integration tests")]
public class SiteTypeTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private readonly List<SiteType> siteTypes = new List<SiteType>
    {
        new SiteType
        {
            Id = "MyFirstType",
            Regions = new List<ContentTypeRegion>
            {
                new ContentTypeRegion
                {
                    Id = "Body",
                    Fields = new List<ContentTypeField>
                    {
                        new ContentTypeField {
                            Id = "Default",
                            Type = "Html"
                        }
                    }
                }
            }
        },
        new SiteType
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
        new SiteType
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
        new SiteType
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
        new SiteType
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

        
        await api.SiteTypes.SaveAsync(siteTypes[0]);
        await api.SiteTypes.SaveAsync(siteTypes[3]);
        await api.SiteTypes.SaveAsync(siteTypes[4]);
    }

    public override async Task DisposeAsync()
    {
        
        var siteTypes = await api.SiteTypes.GetAllAsync();

        foreach (var p in siteTypes)
        {
            await api.SiteTypes.DeleteAsync(p);
        }
    }

    [Fact]
    public void IsCached()
    {
        
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(SiteTypeTestsMemoryCache) ||
            this.GetType() == typeof(SiteTypeTestsDistributedCache));
    }

    [Fact]
    public async Task Add()
    {
        
        await api.SiteTypes.SaveAsync(siteTypes[1]);
    }

    [Fact]
    public async Task GetAll()
    {
        
        var models = await api.SiteTypes.GetAllAsync();

        Assert.NotNull(models);
        Assert.NotEmpty(models);
    }

    [Fact]
    public async Task GetNoneById()
    {
        
        var none = await api.SiteTypes.GetByIdAsync("none-existing-type");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetById()
    {
        
        var model = await api.SiteTypes.GetByIdAsync(siteTypes[0].Id);

        Assert.NotNull(model);
        Assert.Equal(siteTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
    }

    [Fact]
    public async Task Update()
    {
        
        var model = await api.SiteTypes.GetByIdAsync(siteTypes[0].Id);

        Assert.Null(model.Title);

        model.Title = "Updated";

        await api.SiteTypes.SaveAsync(model);
    }

    [Fact]
    public async Task Delete()
    {
        
        var model = await api.SiteTypes.GetByIdAsync(siteTypes[3].Id);

        Assert.NotNull(model);

        await api.SiteTypes.DeleteAsync(model);
    }

    [Fact]
    public async Task DeleteById()
    {
        
        var model = await api.SiteTypes.GetByIdAsync(siteTypes[4].Id);

        Assert.NotNull(model);

        await api.SiteTypes.DeleteAsync(model.Id);
    }
}
