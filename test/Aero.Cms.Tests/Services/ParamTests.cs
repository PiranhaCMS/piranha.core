

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

//[Collection("Integration tests")]
public class ParamTestsMemoryCache(MartenFixture fixture) : ParamTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

//[Collection("Integration tests")]
public class ParamTestsDistributedCache(MartenFixture fixture) : ParamTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

//[Collection("Integration tests")]
public class ParamTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private const string PARAM1 = "MyFirstParam";
    private const string PARAM2 = "MySecondParam";
    private const string PARAM4 = "MyFourthParam";
    private const string PARAM5 = "MyFifthParam";

    private readonly string PARAM1ID = Snowflake.NewId();
    private readonly string PARAM1VALUE = "My first value";

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        
        await api.Params.SaveAsync(new Param
        {
            Id = PARAM1ID,
            Key = PARAM1,
            Value = PARAM1VALUE
        });

        await api.Params.SaveAsync(new Param
        {
            Key = PARAM4,
        });
        await api.Params.SaveAsync(new Param
        {
            Key = PARAM5,
        });
    }

    public override async Task DisposeAsync()
    {
        
        var param = await api.Params.GetAllAsync();

        foreach (var p in param)
        {
            await api.Params.DeleteAsync(p);
        }
    }

    [Fact]
    public void IsCached()
    {
        
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(ParamTestsMemoryCache) ||
            this.GetType() == typeof(ParamTestsDistributedCache));
    }

    [Fact]
    public async Task Add()
    {
        
        await api.Params.SaveAsync(new Param
        {
            Key = PARAM2,
            Value = "My second value"
        });
    }

    [Fact]
    public async Task AddDuplicateKey()
    {
        
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await api.Params.SaveAsync(new Param
            {
                Key = PARAM1,
                Value = "My duplicate value"
            })
        );
    }

    [Fact]
    public async Task AddEmptyKey()
    {
        
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await api.Params.SaveAsync(new Param())
        );
    }

    [Fact]
    public async Task AddTooLongKey()
    {
        
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await api.Params.SaveAsync(new Param
            {
                Key = "IntegerPosuereEratAnteVenenatisDapibusPosuereVelitAliquetNullamQuisRisusEgetUrnaMollisOrnareVelEuLeo",
            })
        );
    }

    [Fact]
    public async Task GetNoneById()
    {
        
        var none = await api.Params.GetByIdAsync(Snowflake.NewId());

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneByKey()
    {
        
        var none = await api.Params.GetByKeyAsync("none-existing-key");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetAll()
    {
        
        var models = await api.Params.GetAllAsync();

        Assert.NotNull(models);
        Assert.NotEmpty(models);
    }

    [Fact]
    public async Task GetById()
    {
        
        var model = await api.Params.GetByIdAsync(PARAM1ID);

        Assert.NotNull(model);
        Assert.Equal(PARAM1, model.Key);
    }

    [Fact]
    public async Task GetByKey()
    {
        
        var model = await api.Params.GetByKeyAsync(PARAM1);

        Assert.NotNull(model);
        Assert.Equal(PARAM1, model.Key);
    }

    [Fact]
    public async Task Update()
    {
        
        var model = await api.Params.GetByIdAsync(PARAM1ID);

        Assert.Equal(PARAM1VALUE, model.Value);

        model.Value = "Updated";

        await api.Params.SaveAsync(model);
    }

    [Fact]
    public async Task Delete()
    {
        
        var model = await api.Params.GetByKeyAsync(PARAM4);

        Assert.NotNull(model);

        await api.Params.DeleteAsync(model);
    }

    [Fact]
    public async Task DeleteById()
    {
        
        var model = await api.Params.GetByKeyAsync(PARAM4);

        Assert.NotNull(model);

        await api.Params.DeleteAsync(model.Id);
    }
}
