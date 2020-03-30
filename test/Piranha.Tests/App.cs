/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Xunit;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests
{
    public class App : BaseTests
    {
        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected override void Init() {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);
            }
        }

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        protected override void Cleanup() { }

        [Fact]
        public void AppInit() {
            using (var api = CreateApi())
            {
                Piranha.App.Init(api);
            }
        }

        [Fact]
        public void Markdown() {
            var str =
                "# This is the title\n" +
                "This is the body";

            str = Piranha.App.Markdown.Transform(str)
                .Replace("\n", "");

            Assert.Equal("<h1>This is the title</h1><p>This is the body</p>", str);
        }

        [Fact]
        public void MarkdownEmptyString() {
            Assert.Equal("", Piranha.App.Markdown.Transform(""));
        }

        [Fact]
        public void MarkdownNullString() {
            Assert.Null(Piranha.App.Markdown.Transform(null));
        }

        [Fact]
        public void Fields() {
            Assert.NotNull(Piranha.App.Fields);
            Assert.NotEmpty(Piranha.App.Fields);
        }

        [Fact]
        public void Modules() {
            Assert.NotNull(Piranha.App.Modules);
        }

        [Fact]
        public void PropertyBindings() {
            Assert.True(Piranha.App.PropertyBindings.HasFlag(System.Reflection.BindingFlags.IgnoreCase));
            Assert.True(Piranha.App.PropertyBindings.HasFlag(System.Reflection.BindingFlags.Public));
            Assert.True(Piranha.App.PropertyBindings.HasFlag(System.Reflection.BindingFlags.Instance));
        }

        private IApi CreateApi()
        {
            var factory = new ContentFactory(services);
            var serviceFactory = new ContentServiceFactory(factory);

            var db = GetDb();

            return new Api(
                factory,
                new AliasRepository(db),
                new ArchiveRepository(db),
                new Piranha.Repositories.MediaRepository(db),
                new PageRepository(db, serviceFactory),
                new PageTypeRepository(db),
                new ParamRepository(db),
                new PostRepository(db, serviceFactory),
                new PostTypeRepository(db),
                new SiteRepository(db, serviceFactory),
                new SiteTypeRepository(db)
            );
        }
    }
}
