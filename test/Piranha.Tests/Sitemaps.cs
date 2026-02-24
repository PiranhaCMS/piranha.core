/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;
using Xunit;

namespace Piranha.Tests;

public class Sitemaps : BaseTests
{
    private Sitemap sitemap;
    private readonly string id_1 = Snowflake.NewId();
    private readonly string id_2 = Snowflake.NewId();

    protected override void Init() {
        sitemap = new Sitemap();

        sitemap.Add(new SitemapItem
        {
            Id = id_1,
            Title = "No navigation title"
        });
        sitemap[0].Items.Add(new SitemapItem
        {
            Id = id_2,
            Title = "Has navigation title",
            NavigationTitle = "Navigation title"
        });
        sitemap[0].Items.Add(new SitemapItem
        {
            Id = Snowflake.NewId()
        });
    }

    protected override void Cleanup()
    {
        //No need to clean up
    }

    [Fact]
    public void GetNoNavigationTitle() {
        var item = sitemap[0];

        Assert.Equal("No navigation title", item.MenuTitle);
    }

    [Fact]
    public void GetNavigationTitle() {
        var item = sitemap[0].Items[0];

        Assert.Equal("Navigation title", item.MenuTitle);
    }

    [Fact]
    public void GetPartial() {
        var partial = sitemap.GetPartial(id_1);

        Assert.NotNull(partial);
        Assert.Equal(2, partial.Count);
        Assert.Equal(id_2, partial[0].Id);
    }

    [Fact]
    public void GetPartialMissing() {
        var partial = sitemap.GetPartial(Snowflake.NewId());

        Assert.Null(partial);
    }

    [Fact]
    public void HasChild() {
        Assert.True(sitemap[0].HasChild(id_2));
    }

    [Fact]
    public void HasChildMissing() {
        Assert.False(sitemap[0].HasChild(Snowflake.NewId()));
    }

    [Fact]
    public void GetBreadcrumb() {
        var crumb = sitemap.GetBreadcrumb(id_2);

        Assert.NotNull(crumb);
        Assert.Equal(2, crumb.Count);
        Assert.Equal(id_1, crumb[0].Id);
        Assert.Equal(id_2, crumb[1].Id);
    }

    [Fact]
    public void GetBreadcrumbMissing() {
        var crumb = sitemap.GetBreadcrumb(Snowflake.NewId());

        Assert.Null(crumb);
    }
}
