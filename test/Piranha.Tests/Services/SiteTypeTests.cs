﻿/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;
using Piranha.Models;

namespace Piranha.Tests.Services;

[Collection("Integration tests")]
public class SiteTypeTestsMemoryCache : SiteTypeTests
{
    public override Task InitializeAsync()
    {
        _cache = new Cache.MemoryCache((IMemoryCache)_services.GetService(typeof(IMemoryCache)));
        return base.InitializeAsync();
    }
}

[Collection("Integration tests")]
public class SiteTypeTestsDistributedCache : SiteTypeTests
{
    public override Task InitializeAsync()
    {
        _cache = new Cache.DistributedCache((IDistributedCache)_services.GetService(typeof(IDistributedCache)));
        return base.InitializeAsync();
    }
}

[Collection("Integration tests")]
public class SiteTypeTests : BaseTestsAsync
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
        using (var api = CreateApi())
        {
            await api.SiteTypes.SaveAsync(siteTypes[0]);
            await api.SiteTypes.SaveAsync(siteTypes[3]);
            await api.SiteTypes.SaveAsync(siteTypes[4]);
        }
    }

    public override async Task DisposeAsync()
    {
        using (var api = CreateApi())
        {
            var siteTypes = await api.SiteTypes.GetAllAsync();

            foreach (var p in siteTypes)
            {
                await api.SiteTypes.DeleteAsync(p);
            }
        }
    }

    [Fact]
    public void IsCached()
    {
        using (var api = CreateApi())
        {
            Assert.Equal(((Api)api).IsCached,
                this.GetType() == typeof(SiteTypeTestsMemoryCache) ||
                this.GetType() == typeof(SiteTypeTestsDistributedCache));
        }
    }

    [Fact]
    public async Task Add()
    {
        using (var api = CreateApi())
        {
            await api.SiteTypes.SaveAsync(siteTypes[1]);
        }
    }

    [Fact]
    public async Task GetAll()
    {
        using (var api = CreateApi())
        {
            var models = await api.SiteTypes.GetAllAsync();

            Assert.NotNull(models);
            Assert.NotEmpty(models);
        }
    }

    [Fact]
    public async Task GetNoneById()
    {
        using (var api = CreateApi())
        {
            var none = await api.SiteTypes.GetByIdAsync("none-existing-type");

            Assert.Null(none);
        }
    }

    [Fact]
    public async Task GetById()
    {
        using (var api = CreateApi())
        {
            var model = await api.SiteTypes.GetByIdAsync(siteTypes[0].Id);

            Assert.NotNull(model);
            Assert.Equal(siteTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
        }
    }

    [Fact]
    public async Task Update()
    {
        using (var api = CreateApi())
        {
            var model = await api.SiteTypes.GetByIdAsync(siteTypes[0].Id);

            Assert.Null(model.Title);

            model.Title = "Updated";

            await api.SiteTypes.SaveAsync(model);
        }
    }

    [Fact]
    public async Task Delete()
    {
        using (var api = CreateApi())
        {
            var model = await api.SiteTypes.GetByIdAsync(siteTypes[3].Id);

            Assert.NotNull(model);

            await api.SiteTypes.DeleteAsync(model);
        }
    }

    [Fact]
    public async Task DeleteById()
    {
        using (var api = CreateApi())
        {
            var model = await api.SiteTypes.GetByIdAsync(siteTypes[4].Id);

            Assert.NotNull(model);

            await api.SiteTypes.DeleteAsync(model.Id);
        }
    }
}
