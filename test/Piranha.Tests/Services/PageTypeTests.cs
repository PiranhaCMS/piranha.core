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
public class PageTypeTestsMemoryCache : PageTypeTests
{
    public override Task InitializeAsync()
    {
        _cache = new Cache.MemoryCache((IMemoryCache)_services.GetService(typeof(IMemoryCache)));
        return base.InitializeAsync();
    }
}

[Collection("Integration tests")]
public class PageTypeTestsDistributedCache : PageTypeTests
{
    public override Task InitializeAsync()
    {
        _cache = new Cache.DistributedCache((IDistributedCache)_services.GetService(typeof(IDistributedCache)));
        return base.InitializeAsync();
    }
}

[Collection("Integration tests")]
public class PageTypeTests : BaseTestsAsync
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
        using (var api = CreateApi())
        {
            await api.PageTypes.SaveAsync(pageTypes[0]);
            await api.PageTypes.SaveAsync(pageTypes[3]);
            await api.PageTypes.SaveAsync(pageTypes[4]);
        }
    }

    public override async Task DisposeAsync()
    {
        using (var api = CreateApi())
        {
            var pageTypes = await api.PageTypes.GetAllAsync();

            foreach (var p in pageTypes)
            {
                await api.PageTypes.DeleteAsync(p);
            }
        }
    }

    [Fact]
    public void IsCached()
    {
        using (var api = CreateApi())
        {
            Assert.Equal(((Api)api).IsCached,
                this.GetType() == typeof(PageTypeTestsMemoryCache) ||
                this.GetType() == typeof(PageTypeTestsDistributedCache));
        }
    }

    [Fact]
    public async Task Add()
    {
        using (var api = CreateApi())
        {
            await api.PageTypes.SaveAsync(pageTypes[1]);
        }
    }

    [Fact]
    public async Task GetAll()
    {
        using (var api = CreateApi())
        {
            var models = await api.PageTypes.GetAllAsync();

            Assert.NotNull(models);
            Assert.NotEmpty(models);
        }
    }

    [Fact]
    public async Task GetNoneById()
    {
        using (var api = CreateApi())
        {
            var none = await api.PageTypes.GetByIdAsync("none-existing-type");

            Assert.Null(none);
        }
    }

    [Fact]
    public async Task GetById()
    {
        using (var api = CreateApi())
        {
            var model = await api.PageTypes.GetByIdAsync(pageTypes[0].Id);

            Assert.NotNull(model);
            Assert.Equal(pageTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
        }
    }

    [Fact]
    public async Task Update()
    {
        using (var api = CreateApi())
        {
            var model = await api.PageTypes.GetByIdAsync(pageTypes[0].Id);

            Assert.Null(model.Title);

            model.Title = "Updated";

            await api.PageTypes.SaveAsync(model);
        }
    }

    [Fact]
    public async Task Delete()
    {
        using (var api = CreateApi())
        {
            var model = await api.PageTypes.GetByIdAsync(pageTypes[3].Id);

            Assert.NotNull(model);

            await api.PageTypes.DeleteAsync(model);
        }
    }

    [Fact]
    public async Task DeleteById()
    {
        using (var api = CreateApi())
        {
            var model = await api.PageTypes.GetByIdAsync(pageTypes[4].Id);

            Assert.NotNull(model);

            await api.PageTypes.DeleteAsync(model.Id);
        }
    }
}
