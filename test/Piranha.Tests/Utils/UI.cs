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

namespace Piranha.Tests.Utils
{
    public class UI : BaseTests
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
        public void FirstParagraphString() {
            var str = "<p>First</p><p>Second</p><p>Third</p>";

            Assert.Equal("<p>First</p>", Piranha.Utils.FirstParagraph(str));
        }

        [Fact]
        public void NoFirstParagraphString() {
            var str = "First,Second,Third";

            Assert.Equal("", Piranha.Utils.FirstParagraph(str));
        }

        [Fact]
        public void FirstParagraphMarkdown() {
            Extend.Fields.MarkdownField field = "First\n\nSecond\n\nThird";

            Assert.Equal("<p>First</p>", Piranha.Utils.FirstParagraph(field));
        }

        [Fact]
        public void NoFirstParagraphMarkdown() {
            Extend.Fields.MarkdownField field = "";

            Assert.Equal("", Piranha.Utils.FirstParagraph(field));
        }

        [Fact]
        public void FirstParagraphHtml() {
            Extend.Fields.HtmlField field = "<p>First</p><p>Second</p><p>Third</p>";

            Assert.Equal("<p>First</p>", Piranha.Utils.FirstParagraph(field));
        }

        [Fact]
        public void NoFirstParagraphHtml() {
            Extend.Fields.HtmlField field = "First,Second,Third";

            Assert.Equal("", Piranha.Utils.FirstParagraph(field));
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
