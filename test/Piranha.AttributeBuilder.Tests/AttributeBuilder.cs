/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using Xunit;

namespace Piranha.AttributeBuilder.Tests
{
    public class AttributeBuilder : IDisposable
    {
        #region Members
        protected Action<Data.DbBuilder> options = o => {
            o.Connection = new SqliteConnection("Filename=./piranha.tests.db");
            o.Migrate = true;
        };
        #endregion

        #region Inner classes
        [PageType(Id = "Simple", Title = "Simple Page Type")]
        public class SimplePageType
        {
            [Region]
            public Extend.Fields.TextField Body { get; set; }
        }

        [PageType(Id = "Complex", Title = "Complex Page Type", Route = "/complex")]
        public class ComplexPageType
        {
            public class BodyRegion
            {
                [Field]
                public Extend.Fields.TextField Title { get; set; }
                [Field]
                public Extend.Fields.TextField Body { get; set; }
            }

            [Region(Title = "Intro", Min = 3, Max = 6)]
            public IList<Extend.Fields.TextField> Slider { get; set; }

            [Region(Title = "Main content")]
            public BodyRegion Content { get; set; }
        }
        #endregion

        public AttributeBuilder() {
            using (var api = new Api(options)) {
                App.Init(api);
            }
        }

        [Fact]
        public void AddSimple() {
            using (var api = new Api(options)) {
                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(SimplePageType));
                builder.Build();

                var type = api.PageTypes.GetById("Simple");

                Assert.NotNull(type);
                Assert.Equal(1, type.Regions.Count);
                Assert.Equal("Body", type.Regions[0].Id);
                Assert.Equal(1, type.Regions[0].Fields.Count);
            }
        }

        [Fact]
        public void AddComplex() {
            using (var api = new Api(options)) {
                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(ComplexPageType));
                builder.Build();

                var type = api.PageTypes.GetById("Complex");

                Assert.NotNull(type);
                Assert.Equal(2, type.Regions.Count);

                Assert.Equal("Slider", type.Regions[0].Id);
                Assert.Equal("Intro", type.Regions[0].Title);
                Assert.Equal(true, type.Regions[0].Collection);
                Assert.Equal(1, type.Regions[0].Fields.Count);

                Assert.Equal("Content", type.Regions[1].Id);
                Assert.Equal("Main content", type.Regions[1].Title);
                Assert.Equal(false, type.Regions[1].Collection);
                Assert.Equal(2, type.Regions[1].Fields.Count);
            }
        }

        public void Dispose() {
            using (var api = new Api(options)) {
                var types = api.PageTypes.GetAll();

                foreach (var t in types)
                    api.PageTypes.Delete(t);
            }
        }
    }
}
