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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Piranha.AttributeBuilder.Tests
{
    public class AttributeBuilder : IDisposable
    {
        protected IServiceProvider services = new ServiceCollection().BuildServiceProvider();

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

            [Region(Title = "Intro")]
            public IList<Extend.Fields.TextField> Slider { get; set; }

            [Region(Title = "Main content")]
            public BodyRegion Content { get; set; }
        }

        [SiteType(Id = "Simple", Title = "Simple Page Type")]
        public class SimpleSiteType
        {
            [Region]
            public Extend.Fields.TextField Body { get; set; }
        }

        [SiteType(Id = "Complex", Title = "Complex Page Type")]
        public class ComplexSiteType
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
        
        public AttributeBuilder() {
            App.Init();
        }

        [Fact]
        public void AddSimple() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), null)) {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), null)) {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), null)) {
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

        [Fact]
        public void AddSimpleSiteType() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), null)) {
                var builder = new SiteTypeBuilder(api)
                    .AddType(typeof(SimpleSiteType));
                builder.Build();

                var type = api.SiteTypes.GetById("Simple");

                Assert.NotNull(type);
                Assert.Equal(1, type.Regions.Count);
                Assert.Equal("Body", type.Regions[0].Id);
                Assert.Equal(1, type.Regions[0].Fields.Count);
            }
        }

        [Fact]
        public void AddComplexSiteType() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), null)) {
                var builder = new SiteTypeBuilder(api)
                    .AddType(typeof(ComplexSiteType));
                builder.Build();

                var type = api.SiteTypes.GetById("Complex");

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
            }
        }

        public void Dispose() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), null)) {
                var types = api.PageTypes.GetAll();

                foreach (var t in types)
                    api.PageTypes.Delete(t);

                var siteTypes = api.SiteTypes.GetAll();

                foreach (var t in siteTypes)
                    api.SiteTypes.Delete(t);
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
