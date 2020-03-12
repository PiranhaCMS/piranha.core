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
    public class PostTypesCached : PostTypes
    {
        protected override void Init() {
            cache = new Cache.SimpleCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class PostTypes : BaseTests
    {
        #region Members
        protected ICache cache;
        private readonly List<PostType> postTypes = new List<PostType>
        {
            new PostType
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
            new PostType
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
            new PostType
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
            new PostType
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
            new PostType
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
                api.PostTypes.Save(postTypes[0]);
                api.PostTypes.Save(postTypes[3]);
                api.PostTypes.Save(postTypes[4]);
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var postTypes = api.PostTypes.GetAll();

                foreach (var p in postTypes)
                    api.PostTypes.Delete(p);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = CreateApi()) {
                Assert.Equal(this.GetType() == typeof(PostTypesCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public void Add() {
            using (var api = CreateApi()) {
                api.PostTypes.Save(postTypes[1]);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = CreateApi()) {
                var models = api.PostTypes.GetAll();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = CreateApi()) {
                var none = api.PostTypes.GetById("none-existing-type");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = CreateApi()) {
                var model = api.PostTypes.GetById(postTypes[0].Id);

                Assert.NotNull(model);
                Assert.Equal(postTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
            }
        }

        [Fact]
        public void Update() {
            using (var api = CreateApi()) {
                var model = api.PostTypes.GetById(postTypes[0].Id);

                Assert.Null(model.Title);

                model.Title = "Updated";

                api.PostTypes.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = CreateApi()) {
                var model = api.PostTypes.GetById(postTypes[3].Id);

                Assert.NotNull(model);

                api.PostTypes.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = CreateApi()) {
                var model = api.PostTypes.GetById(postTypes[4].Id);

                Assert.NotNull(model);

                api.PostTypes.Delete(model.Id);
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
