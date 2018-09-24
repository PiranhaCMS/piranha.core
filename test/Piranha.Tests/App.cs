/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Extend;
using Piranha.Services;
using System.Linq;
using Xunit;

namespace Piranha.Tests
{
    public class App : BaseTests
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
        public void AppInit() {
            Piranha.App.Init();
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
        public void Mapper() {
            Assert.NotNull(Piranha.App.Mapper);
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
    }
}
