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
using System.Linq;
using Xunit;

namespace Piranha.Tests
{
    public class Fields : BaseTests
    {
        [Field(Name = "First")]
        public class MyFirstField : Extend.Fields.SimpleField<string> { }

        [Field(Name = "Second")]
        public class MySecondField : Extend.Fields.SimpleField<string> { }

        /// <summary>
        /// Sets up & initializes the tests.
        /// </summary>
        protected override void Init() {
            using (var api = new Api(options, storage)) {
                Piranha.App.Init(api);
                Piranha.App.Fields.Register<MyFirstField>();
            }
        }

        /// <summary>
        /// Cleans up any possible data and resources
        /// created by the test.
        /// </summary>
        protected override void Cleanup() { }

        [Fact]
        public void GetByType() {
            var field = Piranha.App.Fields.GetByType(typeof(Extend.Fields.HtmlField));

            Assert.NotNull(field);
            Assert.Equal("Html", field.Name);
        }

        [Fact]
        public void GetByTypeName() {
            var field = Piranha.App.Fields.GetByType(typeof(Extend.Fields.HtmlField).FullName);

            Assert.NotNull(field);
            Assert.Equal("Html", field.Name);
        }

        [Fact]
        public void GetByShorthand() {
            var field = Piranha.App.Fields.GetByShorthand("Html");

            Assert.NotNull(field);
            Assert.Equal("Html", field.Name);
        }

        [Fact]
        public void Register() {
            var count = Piranha.App.Fields.Count();

            Piranha.App.Fields.Register<MySecondField>();

            Assert.Equal(count + 1, Piranha.App.Fields.Count());
        }

        [Fact]
        public void UnRegister() {
            var count = Piranha.App.Fields.Count();

            Piranha.App.Fields.UnRegister<MyFirstField>();

            Assert.Equal(count - 1, Piranha.App.Fields.Count());
        }

        [Fact]
        public void Enumerate() {
            foreach (var field in Piranha.App.Fields) {
                Assert.NotNull(field);
            }
        }
    }
}
