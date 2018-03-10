/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Piranha.AttributeBuilder.Tests
{
    public class AttributeBuilder : IDisposable
    {
        #region Inner classes
        [PageType(Id = "Simple", Title = "Simple Page Type")]
        public class SimplePageType
        {
            [Region]
            public Extend.Fields.TextField Body { get; set; }
        }

        [PageType(Id = "Complex", Title = "Complex Page Type")]
        [PageTypeRoute(Title = "Default", Route = "/complex")]
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
            using (var api = new Api(GetDb(), null)) {
                App.Init(api);
            }
        }

        [Fact]
        public void AddSimple() {
            using (var api = new Api(GetDb(), null)) {
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
            using (var api = new Api(GetDb(), null)) {
                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(ComplexPageType));
                builder.Build();

                var type = api.PageTypes.GetById("Complex");

                Assert.NotNull(type);
                Assert.Equal(2, type.Regions.Count);

                Assert.Equal("Slider", type.Regions[0].Id);
                Assert.Equal("Intro", type.Regions[0].Title);
                Assert.True(type.Regions[0].Collection);
                Assert.Equal(1, type.Regions[0].Fields.Count);

                Assert.Equal("Content", type.Regions[1].Id);
                Assert.Equal("Main content", type.Regions[1].Title);
                Assert.False(type.Regions[1].Collection);
                Assert.Equal(2, type.Regions[1].Fields.Count);

                Assert.Equal(1, type.Routes.Count);
                Assert.Equal("/complex", type.Routes[0]);
            }
        }

        [Fact]
        public void DeleteOrphans() {
            using (var api = new Api(GetDb(), null)) {
                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(SimplePageType))
                    .AddType(typeof(ComplexPageType));
                builder.Build();

                Assert.Equal(2, api.PageTypes.GetAll().Count());

                builder = new PageTypeBuilder(api)
                    .AddType(typeof(SimplePageType));
                builder.DeleteOrphans();

                Assert.Single(api.PageTypes.GetAll());
            }
        }

        public void Dispose() {
            using (var api = new Api(GetDb(), null)) {
                var types = api.PageTypes.GetAll();

                foreach (var t in types)
                    api.PageTypes.Delete(t);
            }
        }

        /// <summary>
        /// Gets the test context.
        /// </summary>
        private IDb GetDb() {
            var builder = new DbContextOptionsBuilder<Db>();

            builder.UseSqlite("Filename=./piranha.tests.db");

            return new Db(builder.Options);
        }
    }
}
