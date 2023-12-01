/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using Xunit;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Models;

namespace Piranha.Tests.AttributeBuilder;

[Collection("Integration tests")]
public class TypeBuilderTests : BaseTestsAsync
{
    [ContentGroup(Title = "My Content")]
    public abstract class MyContent<T> : Content<T> where T : Content<T>
    {
        [Region(SortOrder = 1)]
        public Extend.Fields.TextField Body { get; set; }
    }

    [ContentType(Id = "Simple", Title = "Simple Content Type")]
    public class SimpleContentType : MyContent<SimpleContentType>
    {
    }

    [ContentType(Id = "Categorized", Title = "Categorized Content Type")]
    public class CategorizedContentType : MyContent<CategorizedContentType>, ICategorizedContent
    {
        public Taxonomy Category { get; set; }
    }

    [ContentType(Id = "Tagged", Title = "Tagged Content Type")]
    public class TaggedContentType : MyContent<TaggedContentType>, ITaggedContent
    {
        public IList<Taxonomy> Tags { get; set; }
    }

    [ContentType(Id = "Complex", Title = "Complex Content Type", UseExcerpt = false, UsePrimaryImage = false)]
    [ContentTypeEditor(Title = "Custom Editor", Component = "will be replaced", Icon = "will be replaced")]
    [ContentTypeEditor(Title = "Custom Editor", Component = "custom-editor", Icon = "fa fas-fish")]
    [ContentTypeEditor(Title = "Another Editor", Component = "another-editor", Icon = "fa fas-fish")]
    public class ComplexContentType : MyContent<ComplexContentType>
    {
        public class BodyRegion
        {
            [Field(Description = "This is the title")]
            public Extend.Fields.TextField Title { get; set; }
            [Field(Title = "Main Body")]
            public Extend.Fields.TextField Body { get; set; }
        }

        [Region(Title = "Intro", ListTitle = "Default", ListPlaceholder = "Add new item", ListExpand = false, Icon = "fa fas-fish")]
        public IList<Extend.Fields.TextField> Slider { get; set; }

        [Region(Title = "Main content", Description = "This is where you enter the main content")]
        public BodyRegion Content { get; set; }
    }

    [PageType(Id = "Simple", Title = "Simple Page Type")]
    public class SimplePageType : Page<SimplePageType>
    {
        [Region(SortOrder = 1)]
        public Extend.Fields.TextField Body { get; set; }
    }

    [PageType(Id = "Routed", Title = "Routed Page Type")]
    [ContentTypeRoute(Title = "Default", Route = "pageroute")]
    public class RoutedPageType : Page<RoutedPageType>
    {
    }

    [PageType(Id = "Archive", Title = "Archive Page Type", UseBlocks = false, IsArchive = true)]
    [PageTypeArchiveItem(typeof(SimplePostType))]
    public class ArchivePageType : Page<ArchivePageType>
    {
    }

    [PageType(Id = "Complex", Title = "Complex Page Type")]
    [ContentTypeRoute(Title = "Default", Route = "/complex")]
    [ContentTypeEditor(Title = "Custom Editor", Component = "will be replaced", Icon = "will be replaced")]
    [ContentTypeEditor(Title = "Custom Editor", Component = "custom-editor", Icon = "fa fas-fish")]
    [ContentTypeEditor(Title = "Another Editor", Component = "another-editor", Icon = "fa fas-fish")]
    public class ComplexPageType : Page<ComplexPageType>
    {
        public class BodyRegion
        {
            [Field(Description = "This is the title")]
            public Extend.Fields.TextField Title { get; set; }
            [Field(Title = "Main Body")]
            public Extend.Fields.TextField Body { get; set; }
        }

        [Region(Title = "Intro", ListTitle = "Default", ListPlaceholder = "Add new item", ListExpand = false, Icon = "fa fas-fish")]
        public IList<Extend.Fields.TextField> Slider { get; set; }

        [Region(Title = "Main content", Description = "This is where you enter the main content")]
        public BodyRegion Content { get; set; }
    }

    [PostType(Id = "Simple", Title = "Simple Post Type", UseBlocks = false, UsePrimaryImage = false, UseExcerpt = false)]
    public class SimplePostType : Post<SimplePostType>
    {
        [Region]
        public Extend.Fields.TextField Body { get; set; }
    }

    [PostType(Id = "Routed", Title = "Routed Post Type")]
    [ContentTypeRoute(Title = "Default", Route = "postroute")]
    public class RoutedPostType : Post<RoutedPostType>
    {
    }

    [PostType(Id = "Complex", Title = "Complex Post Type")]
    [ContentTypeRoute(Title = "Default", Route = "/complex")]
    [ContentTypeEditor(Title = "Custom Editor", Component = "will be replaced", Icon = "will be replaced")]
    [ContentTypeEditor(Title = "Custom Editor", Component = "custom-editor", Icon = "fa fas-fish")]
    [ContentTypeEditor(Title = "Another Editor", Component = "another-editor", Icon = "fa fas-fish")]
    public class ComplexPostType : Post<ComplexPostType>
    {
        public class BodyRegion
        {
            [Field]
            public Extend.Fields.TextField Title { get; set; }
            [Field]
            public Extend.Fields.TextField Body { get; set; }
        }

        [Region(Title = "Intro")]
        public IList<Extend.Fields.TextField> Slider { get; set; }

        [Region(Title = "Main content")]
        public BodyRegion Content { get; set; }
    }

    [SiteType(Id = "Simple", Title = "Simple Page Type")]
    public class SimpleSiteType : SiteContent<SimpleSiteType>
    {
        [Region(SortOrder = 1)]
        public Extend.Fields.TextField Body { get; set; }
    }

    [SiteType(Id = "Complex", Title = "Complex Page Type")]
    public class ComplexSiteType : SiteContent<ComplexSiteType>
    {
        public class BodyRegion
        {
            [Field]
            public Extend.Fields.TextField Title { get; set; }
            [Field]
            public Extend.Fields.TextField Body { get; set; }
        }

        [Region(Title = "Intro")]
        public IList<Extend.Fields.TextField> Slider { get; set; }

        [Region(Title = "Main content")]
        public BodyRegion Content { get; set; }
    }

    public override Task InitializeAsync()
    {
        return Task.Run(() =>
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);
            }
        });
    }

    public override async Task DisposeAsync()
    {
        using (var api = CreateApi())
        {
            var types = await api.PageTypes.GetAllAsync();
            foreach (var t in types)
            {
                await api.PageTypes.DeleteAsync(t);
            }

            var siteTypes = await api.SiteTypes.GetAllAsync();
            foreach (var t in siteTypes)
            {
                await api.SiteTypes.DeleteAsync(t);
            }

            var contentTypes = await api.ContentTypes.GetAllAsync();
            foreach (var t in contentTypes)
            {
                await api.ContentTypes.DeleteAsync(t);
            }

            var contentGroups = await api.ContentGroups.GetAllAsync();
            foreach (var t in contentGroups)
            {
                await api.ContentGroups.DeleteAsync(t);
            }
        }
    }

    [Fact]
    public async Task AddSimpleContentType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(SimpleContentType))
                .Build();

            var group = await api.ContentGroups.GetByIdAsync("MyContent");
            var type = await api.ContentTypes.GetByIdAsync("Simple");

            Assert.NotNull(group);
            Assert.NotNull(type);

            Assert.Equal("MyContent", type.Group);
            Assert.NotEmpty(type.Regions);
            Assert.Equal("Body", type.Regions[0].Id);
            Assert.NotEmpty(type.Regions[0].Fields);
        }
    }

    [Fact]
    public async Task AddCategorizedContentType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(CategorizedContentType))
                .Build();

            var group = await api.ContentGroups.GetByIdAsync("MyContent");
            var type = await api.ContentTypes.GetByIdAsync("Categorized");

            Assert.NotNull(group);
            Assert.NotNull(type);

            Assert.Equal("MyContent", type.Group);
            //
            // TODO
            //
            // Categories are currently disabled
            //
            // Assert.True(type.UseCategory);
            Assert.False(type.UseTags);
        }
    }

    [Fact]
    public async Task AddTaggedContentType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(TaggedContentType))
                .Build();

            var group = await api.ContentGroups.GetByIdAsync("MyContent");
            var type = await api.ContentTypes.GetByIdAsync("Tagged");

            Assert.NotNull(group);
            Assert.NotNull(type);

            Assert.Equal("MyContent", type.Group);
            Assert.False(type.UseCategory);
            //
            // TODO
            //
            // Tags are currently disabled
            //
            // Assert.True(type.UseTags);
        }
    }

    [Fact]
    public async Task AddComplexContentType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(ComplexContentType))
                .Build();

            var group = await api.ContentGroups.GetByIdAsync("MyContent");
            var type = await api.ContentTypes.GetByIdAsync("Complex");

            Assert.NotNull(group);
            Assert.NotNull(type);
            Assert.Equal("MyContent", type.Group);
            Assert.Equal(3, type.Regions.Count);
            Assert.False(type.UseExcerpt);
            Assert.False(type.UsePrimaryImage);
            Assert.False(type.UseCategory);
            Assert.False(type.UseTags);

            Assert.Equal("Body", type.Regions[0].Id);
            Assert.NotEmpty(type.Regions[0].Fields);


            Assert.Equal("Slider", type.Regions[1].Id);
            Assert.Equal("Intro", type.Regions[1].Title);
            Assert.Equal("Default", type.Regions[1].ListTitleField);
            Assert.Equal("Add new item", type.Regions[1].ListTitlePlaceholder);
            Assert.Equal("fa fas-fish", type.Regions[1].Icon);
            Assert.False(type.Regions[1].ListExpand);
            Assert.True(type.Regions[1].Collection);
            Assert.NotEmpty(type.Regions[1].Fields);

            Assert.Equal("Content", type.Regions[2].Id);
            Assert.Equal("Main content", type.Regions[2].Title);
            Assert.Equal("This is where you enter the main content", type.Regions[2].Description);
            Assert.False(type.Regions[2].Collection);
            Assert.Equal(2, type.Regions[2].Fields.Count);
            Assert.Equal("Title", type.Regions[2].Fields[0].Id);
            Assert.Equal("This is the title", type.Regions[2].Fields[0].Description);
            Assert.Equal("Body", type.Regions[2].Fields[1].Id);
            Assert.Equal("Main Body", type.Regions[2].Fields[1].Title);

            Assert.Equal(2, type.CustomEditors.Count);
            Assert.Equal("Custom Editor", type.CustomEditors[0].Title);
            Assert.Equal("custom-editor", type.CustomEditors[0].Component);
            Assert.Equal("fa fas-fish", type.CustomEditors[0].Icon);
        }
    }

    [Fact]
    public async Task AddMultipleContentTypes()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(SimpleContentType))
                .AddType(typeof(ComplexContentType))
                .Build();

            var groupCount = (await api.ContentGroups.GetAllAsync()).Count();
            var typeCount = (await api.ContentTypes.GetAllAsync()).Count();

            Assert.Equal(1, groupCount);
            Assert.Equal(2, typeCount);
        }
    }

    [Fact]
    public async Task AddSimplePageType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(SimplePageType))
                .Build();

            var type = await api.PageTypes.GetByIdAsync("Simple");

            Assert.NotNull(type);
            Assert.True(type.UseBlocks);
            Assert.NotEmpty(type.Regions);
            Assert.Equal("Body", type.Regions[0].Id);
            Assert.NotEmpty(type.Regions[0].Fields);
        }
    }

    [Fact]
    public async Task AddForwardSlashToPageRoutes()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(RoutedPageType))
                .Build();

            var type = await api.PageTypes.GetByIdAsync("Routed");
            var types = await api.PageTypes.GetAllAsync();

            Assert.NotNull(type);

            Assert.NotEmpty(type.Routes);
            Assert.StartsWith("/", type.Routes[0]);
        }
    }

    [Fact]
    public async Task AddArchivePageType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(ArchivePageType))
                .Build();

            var type = await api.PageTypes.GetByIdAsync("Archive");

            Assert.NotNull(type);
            Assert.True(type.IsArchive);
            Assert.False(type.UseBlocks);
            Assert.NotEmpty(type.ArchiveItemTypes);
        }
    }

    [Fact]
    public async Task AddComplexPageType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(ComplexPageType))
                .Build();

            var type = await api.PageTypes.GetByIdAsync("Complex");

            Assert.NotNull(type);
            Assert.Equal(2, type.Regions.Count);

            Assert.Equal("Slider", type.Regions[0].Id);
            Assert.Equal("Intro", type.Regions[0].Title);
            Assert.Equal("Default", type.Regions[0].ListTitleField);
            Assert.Equal("Add new item", type.Regions[0].ListTitlePlaceholder);
            Assert.Equal("fa fas-fish", type.Regions[0].Icon);
            Assert.False(type.Regions[0].ListExpand);
            Assert.True(type.Regions[0].Collection);
            Assert.NotEmpty(type.Regions[0].Fields);

            Assert.Equal("Content", type.Regions[1].Id);
            Assert.Equal("Main content", type.Regions[1].Title);
            Assert.Equal("This is where you enter the main content", type.Regions[1].Description);
            Assert.False(type.Regions[1].Collection);
            Assert.Equal(2, type.Regions[1].Fields.Count);
            Assert.Equal("Title", type.Regions[1].Fields[0].Id);
            Assert.Equal("This is the title", type.Regions[1].Fields[0].Description);
            Assert.Equal("Body", type.Regions[1].Fields[1].Id);
            Assert.Equal("Main Body", type.Regions[1].Fields[1].Title);

            Assert.NotEmpty(type.Routes);
            Assert.Equal("/complex", type.Routes[0]);

            Assert.Equal(2, type.CustomEditors.Count);
            Assert.Equal("Custom Editor", type.CustomEditors[0].Title);
            Assert.Equal("custom-editor", type.CustomEditors[0].Component);
            Assert.Equal("fa fas-fish", type.CustomEditors[0].Icon);
        }
    }

    [Fact]
    public async Task AddSimplePostType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(SimplePostType))
                .Build();

            var type = await api.PostTypes.GetByIdAsync("Simple");

            Assert.NotNull(type);
            Assert.False(type.UseBlocks);
            Assert.False(type.UseExcerpt);
            Assert.False(type.UsePrimaryImage);
            Assert.NotEmpty(type.Regions);
            Assert.Equal("Body", type.Regions[0].Id);
            Assert.NotEmpty(type.Regions[0].Fields);
        }
    }

    [Fact]
    public async Task AddForwardSlashToPostRoutes()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(RoutedPostType))
                .Build();

            var type = await api.PostTypes.GetByIdAsync("Routed");

            Assert.NotNull(type);

            Assert.NotEmpty(type.Routes);
            Assert.StartsWith("/", type.Routes[0]);
        }
    }

    [Fact]
    public async Task AddComplexPostType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(ComplexPostType))
                .Build();

            var type = await api.PostTypes.GetByIdAsync("Complex");

            Assert.NotNull(type);
            Assert.True(type.UseBlocks);
            Assert.True(type.UseExcerpt);
            Assert.True(type.UsePrimaryImage);
            Assert.Equal(2, type.Regions.Count);

            Assert.Equal("Slider", type.Regions[0].Id);
            Assert.Equal("Intro", type.Regions[0].Title);
            Assert.True(type.Regions[0].Collection);
            Assert.NotEmpty(type.Regions[0].Fields);

            Assert.Equal("Content", type.Regions[1].Id);
            Assert.Equal("Main content", type.Regions[1].Title);
            Assert.False(type.Regions[1].Collection);
            Assert.Equal(2, type.Regions[1].Fields.Count);

            Assert.NotEmpty(type.Routes);
            Assert.Equal("/complex", type.Routes[0]);

            Assert.Equal(2, type.CustomEditors.Count);
            Assert.Equal("Custom Editor", type.CustomEditors[0].Title);
            Assert.Equal("custom-editor", type.CustomEditors[0].Component);
            Assert.Equal("fa fas-fish", type.CustomEditors[0].Icon);
        }
    }

    [Fact]
    public async Task DeleteOrphans()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(SimplePageType))
                .AddType(typeof(ComplexPageType))
                .Build();

            Assert.Equal(2, (await api.PageTypes.GetAllAsync()).Count());

            new ContentTypeBuilder(api)
                .AddType(typeof(SimplePageType))
                .Build()
                .DeleteOrphans();

            Assert.Single(await api.PageTypes.GetAllAsync());
        }
    }

    [Fact]
    public async Task AddSimpleSiteType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(SimpleSiteType))
                .Build();

            var type = await api.SiteTypes.GetByIdAsync("Simple");

            Assert.NotNull(type);
            Assert.NotEmpty(type.Regions);
            Assert.Equal("Body", type.Regions[0].Id);
            Assert.NotEmpty(type.Regions[0].Fields);
        }
    }

    [Fact]
    public async Task AddComplexSiteType()
    {
        using (var api = CreateApi())
        {
            new ContentTypeBuilder(api)
                .AddType(typeof(ComplexSiteType))
                .Build();

            var type = await api.SiteTypes.GetByIdAsync("Complex");

            Assert.NotNull(type);
            Assert.Equal(2, type.Regions.Count);

            Assert.Equal("Slider", type.Regions[0].Id);
            Assert.Equal("Intro", type.Regions[0].Title);
            Assert.True(type.Regions[0].Collection);
            Assert.NotEmpty(type.Regions[0].Fields);

            Assert.Equal("Content", type.Regions[1].Id);
            Assert.Equal("Main content", type.Regions[1].Title);
            Assert.False(type.Regions[1].Collection);
            Assert.Equal(2, type.Regions[1].Fields.Count);
        }
    }
}
