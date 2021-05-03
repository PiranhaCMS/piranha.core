/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.Threading.Tasks;
using Xunit;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class ContentTests : BaseTestsAsync
    {
        private readonly Guid ID_1 = Guid.NewGuid();
        private readonly Guid ID_2 = Guid.NewGuid();
        private readonly Guid ID_3 = Guid.NewGuid();

        private readonly Guid ID_LANG = Guid.NewGuid();

        [ContentGroup(Id = "MyContentGroup", Title = "My content group")]
        public abstract class MyContentGroup<T> : Content<T> where T : MyContentGroup<T>
        {
        }

        [ContentType(Id = "MyContent", Title = "My content")]
        public class MyContent : MyContentGroup<MyContent>
        {
            [Region]
            public HtmlField MainDescription { get; set; }
        }

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

                // Add the content type
                var builder = new ContentTypeBuilder(api)
                    .AddType(typeof(MyContent));
                await builder.BuildAsync();

                // Add a secondary language
                await api.Languages.SaveAsync(new Language
                {
                    Id = ID_LANG,
                    Title = "Second Language",
                    Culture = "sv-SE"
                });

                // Add some default content
                var content1 = await MyContent.CreateAsync(api);
                content1.Id = ID_1;
                content1.Title = "My first content";
                content1.Excerpt = "My first excerpt";
                content1.MainDescription = "My first description";

                await api.Content.SaveAsync(content1);

                var content2 = await MyContent.CreateAsync(api);
                content2.Id = ID_2;
                content2.Title = "My second content";
                content2.Excerpt = "My second excerpt";
                content2.MainDescription = "My second description";

                await api.Content.SaveAsync(content2);

                var content3 = await MyContent.CreateAsync(api);
                content3.Id = ID_3;
                content3.Title = "My third content";
                content3.Excerpt = "My third excerpt";
                content3.MainDescription = "My third description";

                await api.Content.SaveAsync(content3);

                // Now let's translate content 1
                content1.Title = "Mitt första innehåll";
                content1.Excerpt = "Min första sammanfattning";
                content1.MainDescription = "Min första beskrivning";

                await api.Content.SaveAsync(content1, ID_LANG);
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                // Delete added content
                var content = await api.Content.GetAllAsync();
                foreach (var c in content)
                {
                    await api.Content.DeleteAsync(c);
                }

                // Delete added content groups
                var groups = await api.ContentGroups.GetAllAsync();
                foreach (var g in groups)
                {
                    await api.ContentGroups.DeleteAsync(g);
                }

                // Delete added language
                await api.Languages.DeleteAsync(ID_LANG);
            }
        }

        [Fact]
        public async Task GetById()
        {
            using (var api = CreateApi())
            {
                var content = await api.Content.GetByIdAsync<MyContent>(ID_1);

                Assert.NotNull(content);
                Assert.Equal("My first content", content.Title);
            }
        }

        [Fact]
        public async Task GetTranslationById()
        {
            using (var api = CreateApi())
            {
                var content = await api.Content.GetByIdAsync<MyContent>(ID_1, ID_LANG);

                Assert.NotNull(content);
                Assert.Equal("Mitt första innehåll", content.Title);
            }
        }

        [Fact]
        public async Task GetTranslatedStatus()
        {
            using (var api = CreateApi())
            {
                var status = await api.Content.GetTranslationStatusByIdAsync(ID_1);

                Assert.NotNull(status);

                Assert.True(status.IsUpToDate);
                Assert.Equal(1, status.UpToDateCount);
                Assert.Equal(1, status.TotalCount);
            }
        }

        [Fact]
        public async Task GetUntranslatedStatus()
        {
            using (var api = CreateApi())
            {
                var status = await api.Content.GetTranslationStatusByIdAsync(ID_2);

                Assert.NotNull(status);

                Assert.False(status.IsUpToDate);
                Assert.Equal(0, status.UpToDateCount);
                Assert.Equal(1, status.TotalCount);
            }
        }

        [Fact]
        public async Task GetTranslationSummary()
        {
            using (var api = CreateApi())
            {
                var summary = await api.Content.GetTranslationStatusByGroupAsync("MyContentGroup");

                Assert.NotNull(summary);

                Assert.False(summary.IsUpToDate);
                Assert.Equal(1, summary.UpToDateCount);
                Assert.Equal(3, summary.TotalCount);
            }
        }
    }
}