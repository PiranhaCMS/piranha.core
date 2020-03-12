/*
 * Copyright (c) 2018-2019 Håkan Edling
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
    public class SiteTypesCached : SiteTypes
    {
        protected override void Init()
        {
            cache = new Cache.SimpleCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class SiteTypes : BaseTests
    {
        protected ICache cache;
        private readonly List<SiteType> siteTypes = new List<SiteType>
        {
            new SiteType
            {
                Id = "MyFirstType",
                Regions = new List<RegionType>
                {
                    new RegionType
                    {
                        Id = "Body",
                        Fields = new List<FieldType>
                        {
                            new FieldType {
                                Id = "Default",
                                Type = "Html"
                            }
                        }
                    }
                }
            },
            new SiteType
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
            new SiteType
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
            new SiteType
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
            new SiteType
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

        protected override void Init()
        {
            using (var api = CreateApi())
            {
                api.SiteTypes.Save(siteTypes[0]);
                api.SiteTypes.Save(siteTypes[3]);
                api.SiteTypes.Save(siteTypes[4]);
            }
        }

        protected override void Cleanup()
        {
            using (var api = CreateApi())
            {
                var siteTypes = api.SiteTypes.GetAll();

                foreach (var p in siteTypes)
                {
                    api.SiteTypes.Delete(p);
                }
            }
        }

        [Fact]
        public void IsCached()
        {
            using (var api = CreateApi())
            {
                Assert.Equal(this.GetType() == typeof(SiteTypesCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public void Add()
        {
            using (var api = CreateApi())
            {
                api.SiteTypes.Save(siteTypes[1]);
            }
        }

        [Fact]
        public void GetAll()
        {
            using (var api = CreateApi())
            {
                var models = api.SiteTypes.GetAll();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetNoneById()
        {
            using (var api = CreateApi())
            {
                var none = api.SiteTypes.GetById("none-existing-type");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetById()
        {
            using (var api = CreateApi())
            {
                var model = api.SiteTypes.GetById(siteTypes[0].Id);

                Assert.NotNull(model);
                Assert.Equal(siteTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
            }
        }

        [Fact]
        public void Update()
        {
            using (var api = CreateApi())
            {
                var model = api.SiteTypes.GetById(siteTypes[0].Id);

                Assert.Null(model.Title);

                model.Title = "Updated";

                api.SiteTypes.Save(model);
            }
        }

        [Fact]
        public void Delete()
        {
            using (var api = CreateApi())
            {
                var model = api.SiteTypes.GetById(siteTypes[3].Id);

                Assert.NotNull(model);

                api.SiteTypes.Delete(model);
            }
        }

        [Fact]
        public void DeleteById()
        {
            using (var api = CreateApi())
            {
                var model = api.SiteTypes.GetById(siteTypes[4].Id);

                Assert.NotNull(model);

                api.SiteTypes.Delete(model.Id);
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
