/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.Caching.Memory;
using Xunit;

namespace Piranha.Tests;

public class MemCache : BaseTestsAsync
{
    private readonly string id1 = Guid.NewGuid().ToString();
    private readonly string id2 = Guid.NewGuid().ToString();
    private readonly string id3 = Guid.NewGuid().ToString();
    private readonly string val1 = "My first value";
    private readonly string val2 = "My second value";
    private readonly string val3 = "My third value";
    private readonly string val4 = "My fourth value";

    public override Task InitializeAsync()
    {
        return Task.Run(() => {
            _cache = new Cache.MemoryCache((IMemoryCache)_services.GetService(typeof(IMemoryCache)));

            _cache.Set(id1, val1);
            _cache.Set(id2, val2);
        });
    }

    public override Task DisposeAsync()
    {
        return Task.Run(() => {});
    }

    [Fact]
    public void AddEntry()
    {
        _cache.Set(id3, val3);
    }

    [Fact]
    public void GetEntry()
    {
        var val = _cache.Get<string>(id2);

        Assert.NotNull(val);
        Assert.Equal(val2, val);
    }

    [Fact]
    public void UpdateEntry() {
        _cache.Set(id2, val4);

        var val = _cache.Get<string>(id2);

        Assert.NotNull(val);
        Assert.Equal(val4, val);
    }

    [Fact]
    public void RemoveEntry()
    {
        _cache.Remove(id1);

        var val = _cache.Get<string>(id1);

        Assert.Null(val);
    }
}
