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

namespace Piranha.Tests.Repositories;

[Collection("Integration tests")]
public class PostTypeTestsMemoryCache : PostTypeTests
{
    public override Task InitializeAsync()
    {
        _cache = new Cache.MemoryCache((IMemoryCache)_services.GetService(typeof(IMemoryCache)));
        return base.InitializeAsync();
    }
}

[Collection("Integration tests")]
public class PostTypeTestsDistributedCache : PostTypeTests
{
    public override Task InitializeAsync()
    {
        _cache = new Cache.DistributedCache((IDistributedCache)_services.GetService(typeof(IDistributedCache)));
        return base.InitializeAsync();
    }
}

[Collection("Integration tests")]
public class PostTypeTests : BaseTestsAsync
{
    private readonly List<PostType> postTypes = new List<PostType>
    {
        new PostType
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
        new PostType
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
        new PostType
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
        new PostType
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
        new PostType
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
            await api.PostTypes.SaveAsync(postTypes[0]);
            await api.PostTypes.SaveAsync(postTypes[3]);
            await api.PostTypes.SaveAsync(postTypes[4]);
        }
    }

    public override async Task DisposeAsync()
    {
        using (var api = CreateApi())
        {
            var postTypes = await api.PostTypes.GetAllAsync();

            foreach (var p in postTypes)
            {
                await api.PostTypes.DeleteAsync(p);
            }
        }
    }

    [Fact]
    public void IsCached()
    {
        using (var api = CreateApi())
        {
            Assert.Equal(((Api)api).IsCached,
                this.GetType() == typeof(PostTypeTestsMemoryCache) ||
                this.GetType() == typeof(PostTypeTestsDistributedCache));
        }
    }

    [Fact]
    public async Task Add()
    {
        using (var api = CreateApi())
        {
            await api.PostTypes.SaveAsync(postTypes[1]);
        }
    }

    [Fact]
    public async Task GetAll()
    {
        using (var api = CreateApi())
        {
            var models = await api.PostTypes.GetAllAsync();

            Assert.NotNull(models);
            Assert.NotEmpty(models);
        }
    }

    [Fact]
    public async Task GetNoneById()
    {
        using (var api = CreateApi())
        {
            var none = await api.PostTypes.GetByIdAsync("none-existing-type");

            Assert.Null(none);
        }
    }

    [Fact]
    public async Task GetById()
    {
        using (var api = CreateApi())
        {
            var model = await api.PostTypes.GetByIdAsync(postTypes[0].Id);

            Assert.NotNull(model);
            Assert.Equal(postTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
        }
    }

    [Fact]
    public async Task Update()
    {
        using (var api = CreateApi())
        {
            var model = await api.PostTypes.GetByIdAsync(postTypes[0].Id);

            Assert.Null(model.Title);

            model.Title = "Updated";

            await api.PostTypes.SaveAsync(model);
        }
    }

    [Fact]
    public async Task Delete()
    {
        using (var api = CreateApi())
        {
            var model = await api.PostTypes.GetByIdAsync(postTypes[3].Id);

            Assert.NotNull(model);

            await api.PostTypes.DeleteAsync(model);
        }
    }

    [Fact]
    public async Task DeleteById()
    {
        using (var api = CreateApi())
        {
            var model = await api.PostTypes.GetByIdAsync(postTypes[4].Id);

            Assert.NotNull(model);

            await api.PostTypes.DeleteAsync(model.Id);
        }
    }
}
