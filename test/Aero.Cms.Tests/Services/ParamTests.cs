

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

[Collection("Integration tests")]
public class ParamTestsMemoryCache : ParamTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

[Collection("Integration tests")]
public class ParamTestsDistributedCache : ParamTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

[Collection("Integration tests")]
public class ParamTests : BaseTestsAsync
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

        using var api = CreateApi();
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
        using var api = CreateApi();
        var param = await api.Params.GetAllAsync();

        foreach (var p in param)
        {
            await api.Params.DeleteAsync(p);
        }
    }

    [Fact]
    public void IsCached()
    {
        using var api = CreateApi();
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(ParamTestsMemoryCache) ||
            this.GetType() == typeof(ParamTestsDistributedCache));
    }

    [Fact]
    public async Task Add()
    {
        using var api = CreateApi();
        await api.Params.SaveAsync(new Param
        {
            Key = PARAM2,
            Value = "My second value"
        });
    }

    [Fact]
    public async Task AddDuplicateKey()
    {
        using var api = CreateApi();
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
        using var api = CreateApi();
        await Assert.ThrowsAsync<ValidationException>(async () =>
            await api.Params.SaveAsync(new Param())
        );
    }

    [Fact]
    public async Task AddTooLongKey()
    {
        using var api = CreateApi();
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
        using var api = CreateApi();
        var none = await api.Params.GetByIdAsync(Snowflake.NewId());

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneByKey()
    {
        using var api = CreateApi();
        var none = await api.Params.GetByKeyAsync("none-existing-key");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetAll()
    {
        using var api = CreateApi();
        var models = await api.Params.GetAllAsync();

        Assert.NotNull(models);
        Assert.NotEmpty(models);
    }

    [Fact]
    public async Task GetById()
    {
        using var api = CreateApi();
        var model = await api.Params.GetByIdAsync(PARAM1ID);

        Assert.NotNull(model);
        Assert.Equal(PARAM1, model.Key);
    }

    [Fact]
    public async Task GetByKey()
    {
        using var api = CreateApi();
        var model = await api.Params.GetByKeyAsync(PARAM1);

        Assert.NotNull(model);
        Assert.Equal(PARAM1, model.Key);
    }

    [Fact]
    public async Task Update()
    {
        using var api = CreateApi();
        var model = await api.Params.GetByIdAsync(PARAM1ID);

        Assert.Equal(PARAM1VALUE, model.Value);

        model.Value = "Updated";

        await api.Params.SaveAsync(model);
    }

    [Fact]
    public async Task Delete()
    {
        using var api = CreateApi();
        var model = await api.Params.GetByKeyAsync(PARAM4);

        Assert.NotNull(model);

        await api.Params.DeleteAsync(model);
    }

    [Fact]
    public async Task DeleteById()
    {
        using var api = CreateApi();
        var model = await api.Params.GetByKeyAsync(PARAM4);

        Assert.NotNull(model);

        await api.Params.DeleteAsync(model.Id);
    }
}
