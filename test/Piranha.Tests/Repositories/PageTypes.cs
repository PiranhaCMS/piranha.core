/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System.Collections.Generic;
using Xunit;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class PageTypesCached : PageTypes
    {
        protected override void Init() {
            cache = new Cache.SimpleCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class PageTypes : BaseTests
    {
        #region Members
        protected ICache cache;
        private readonly List<PageType> pageTypes = new List<PageType>
        {
            new PageType
            {
                Id = "MyFirstType",
                Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType
                            {
                                Id = "Default",
                                Type = "Html"
                            }
                        }
                    }
                }
            },
            new PageType
            {
                Id = "MySecondType",
                Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType
                            {
                                Id = "Default",
                                Type = "Text"
                            }
                        }
                    }
                }
            },
            new PageType
            {
                Id = "MyThirdType",
                Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType
                            {
                                Id = "Default",
                                Type = "Image"
                            }
                        }
                    }
                }
            },
            new PageType
            {
                Id = "MyFourthType",
                Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType
                            {
                                Id = "Default",
                                Type = "String"
                            }
                        }
                    }
                }
            },
            new PageType
            {
                Id = "MyFifthType",
                Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType
                            {
                                Id = "Default",
                                Type = "Text"
                            }
                        }
                    }
                }
            }
        };
        #endregion

        protected override void Init() {
            using (var api = CreateApi()) {
                api.PageTypes.Save(pageTypes[0]);
                api.PageTypes.Save(pageTypes[3]);
                api.PageTypes.Save(pageTypes[4]);
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var pageTypes = api.PageTypes.GetAll();

                foreach (var p in pageTypes)
                    api.PageTypes.Delete(p);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = CreateApi()) {
                Assert.Equal(this.GetType() == typeof(PageTypesCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public void Add() {
            using (var api = CreateApi()) {
                api.PageTypes.Save(pageTypes[1]);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = CreateApi()) {
                var models = api.PageTypes.GetAll();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = CreateApi()) {
                var none = api.PageTypes.GetById("none-existing-type");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = CreateApi()) {
                var model = api.PageTypes.GetById(pageTypes[0].Id);

                Assert.NotNull(model);
                Assert.Equal(pageTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
            }
        }

        [Fact]
        public void Update() {
            using (var api = CreateApi()) {
                var model = api.PageTypes.GetById(pageTypes[0].Id);

                Assert.Null(model.Title);

                model.Title = "Updated";

                api.PageTypes.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = CreateApi()) {
                var model = api.PageTypes.GetById(pageTypes[3].Id);

                Assert.NotNull(model);

                api.PageTypes.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = CreateApi()) {
                var model = api.PageTypes.GetById(pageTypes[4].Id);

                Assert.NotNull(model);

                api.PageTypes.Delete(model.Id);
            }
        }

        private IApi CreateApi()
        {
            var factory = new LegacyContentFactory(services);
            var serviceFactory = new ContentServiceFactory(factory);

            var db = GetDb();

            return new Api(
                factory,
                new AliasRepository(db),
                new ArchiveRepository(db),
                new ContentTypeRepository(db),
                new Piranha.Repositories.MediaRepository(db),
                new PageRepository(db, serviceFactory),
                new PageTypeRepository(db),
                new ParamRepository(db),
                new PostRepository(db, serviceFactory),
                new PostTypeRepository(db),
                new SiteRepository(db, serviceFactory),
                new SiteTypeRepository(db),
                cache: cache,
                storage: storage
            );
        }
    }
}
