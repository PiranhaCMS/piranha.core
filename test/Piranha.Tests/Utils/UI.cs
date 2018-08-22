/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Services;
using System;
using Xunit;

namespace Piranha.Tests.Utils
{
    public class UI : BaseTests
    {
        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected override void Init() {
            Piranha.App.Init();
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
    }
}
