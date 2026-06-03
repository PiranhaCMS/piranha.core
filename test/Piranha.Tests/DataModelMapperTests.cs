/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Data;
using Piranha.Data.EF.Mapping;
using Xunit;

namespace Piranha.Tests;

public class DataModelMapperTests
{
    private sealed class TestPage : Models.Page<TestPage> { }

    private sealed class TestPost : Models.Post<TestPost> { }

    private sealed class TestSiteContent : Models.SiteContent<TestSiteContent> { }

    [Fact]
    public void MapsPageDataToModel()
    {
        var id = Guid.NewGuid();
        var imageId = Guid.NewGuid();
        var page = new Page
        {
            Id = id,
            Title = "Page title",
            PageTypeId = "PageType",
            Slug = "page-slug",
            PrimaryImageId = imageId,
            Excerpt = "Excerpt",
            MetaIndex = true,
            MetaFollow = false,
            SortOrder = 2,
            IsHidden = true,
            Permissions = { new PagePermission { Permission = "Secret" } }
        };
        var model = new TestPage();

        DataModelMapper.MapToModel(page, model);

        Assert.Equal(id, model.Id);
        Assert.Equal("Page title", model.Title);
        Assert.Equal("PageType", model.TypeId);
        Assert.Equal("page-slug", model.Slug);
        Assert.Equal(imageId, model.PrimaryImage.Id);
        Assert.Equal("Excerpt", model.Excerpt);
        Assert.True(model.MetaIndex);
        Assert.False(model.MetaFollow);
        Assert.True(model.IsHidden);
        Assert.Empty(model.Permissions);
    }

    [Fact]
    public void MapsPageModelToDataWithoutOverwritingManagedCollections()
    {
        var created = new DateTime(2024, 1, 1);
        var lastModified = new DateTime(2024, 2, 1);
        var imageId = Guid.NewGuid();
        var page = new Page
        {
            Created = created,
            LastModified = lastModified,
            ContentType = "Existing",
            Permissions = { new PagePermission { Permission = "Keep" } }
        };
        var model = new TestPage
        {
            Id = Guid.NewGuid(),
            Title = "Updated",
            TypeId = "PageType",
            Slug = "updated",
            PrimaryImage = new Extend.Fields.ImageField { Id = imageId },
            Created = DateTime.Now,
            LastModified = DateTime.Now,
            Permissions = { "Ignored" }
        };

        DataModelMapper.MapToData(model, page);

        Assert.Equal(model.Id, page.Id);
        Assert.Equal("Updated", page.Title);
        Assert.Equal("PageType", page.PageTypeId);
        Assert.Equal("updated", page.Slug);
        Assert.Equal(imageId, page.PrimaryImageId);
        Assert.Equal(created, page.Created);
        Assert.Equal(lastModified, page.LastModified);
        Assert.Equal("Existing", page.ContentType);
        Assert.Single(page.Permissions);
        Assert.Equal("Keep", page.Permissions[0].Permission);
    }

    [Fact]
    public void MapsPostTaxonomies()
    {
        var categoryId = Guid.NewGuid();
        var tagId = Guid.NewGuid();
        var post = new Post
        {
            Id = Guid.NewGuid(),
            Title = "Post title",
            PostTypeId = "PostType",
            BlogId = Guid.NewGuid(),
            Category = new Category
            {
                Id = categoryId,
                Title = "Category",
                Slug = "category"
            },
            Tags =
            {
                new PostTag
                {
                    TagId = tagId,
                    Tag = new Tag
                    {
                        Id = tagId,
                        Title = "Tag",
                        Slug = "tag"
                    }
                }
            }
        };
        var model = new TestPost();

        DataModelMapper.MapToModel(post, model);

        Assert.Equal("PostType", model.TypeId);
        Assert.Equal(categoryId, model.Category.Id);
        Assert.Equal(Models.TaxonomyType.Category, model.Category.Type);
        Assert.Single(model.Tags);
        Assert.Equal(tagId, model.Tags[0].Id);
        Assert.Equal(Models.TaxonomyType.Tag, model.Tags[0].Type);
    }

    [Fact]
    public void MapsSitemapPermissions()
    {
        var page = new Page
        {
            Id = Guid.NewGuid(),
            Slug = "child",
            SortOrder = 1,
            Permissions =
            {
                new PagePermission { Permission = "Admin" },
                new PagePermission { Permission = "Editor" }
            }
        };

        var item = DataModelMapper.Map(page);

        Assert.Equal("/child", item.Permalink);
        Assert.Equal(new[] { "Admin", "Editor" }, item.Permissions);
    }

    [Fact]
    public void MapsSiteModelToDataWithoutOverwritingSiteSettings()
    {
        var site = new Site
        {
            SiteTypeId = "ExistingType",
            InternalId = "main",
            Hostnames = "example.com",
            IsDefault = true,
            ContentLastModified = new DateTime(2024, 1, 1)
        };
        var model = new TestSiteContent
        {
            Id = Guid.NewGuid(),
            Title = "Site content",
            TypeId = "IgnoredType"
        };

        DataModelMapper.MapToData(model, site);

        Assert.Equal(model.Id, site.Id);
        Assert.Equal("Site content", site.Title);
        Assert.Equal("ExistingType", site.SiteTypeId);
        Assert.Equal("main", site.InternalId);
        Assert.Equal("example.com", site.Hostnames);
        Assert.True(site.IsDefault);
        Assert.Equal(new DateTime(2024, 1, 1), site.ContentLastModified);
    }
}
