/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Models;
using Piranha.Services;
using System.Collections.Generic;
using System.Linq;
using Xunit;

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
        private List<PostType> postTypes = new List<PostType>() {
            new PostType() {
                Id = "MyFirstType",
                Regions = new List<RegionType>() {
                    new RegionType() {
                        Id = "Body",
                        Fields = new List<FieldType>() {
                            new FieldType() {
                                Id = "Default",
                                Type = "Html"
                            }
                        }
                    }
                }
            },
            new PostType() {
                Id = "MySecondType",
                Regions = new List<RegionType>() {
                    new RegionType() {
                        Id = "Body",
                        Fields = new List<FieldType>() {
                            new FieldType() {
                                Id = "Default",
                                Type = "Text"
                            }
                        }
                    }
                }
            },
            new PostType() {
                Id = "MyThirdType",
                Regions = new List<RegionType>() {
                    new RegionType() {
                        Id = "Body",
                        Fields = new List<FieldType>() {
                            new FieldType() {
                                Id = "Default",
                                Type = "Image"
                            }
                        }
                    }
                }
            },
            new PostType() {
                Id = "MyFourthType",
                Regions = new List<RegionType>() {
                    new RegionType() {
                        Id = "Body",
                        Fields = new List<FieldType>() {
                            new FieldType() {
                                Id = "Default",
                                Type = "String"
                            }
                        }
                    }
                }
            },
            new PostType() {
                Id = "MyFifthType",
                Regions = new List<RegionType>() {
                    new RegionType() {
                        Id = "Body",
                        Fields = new List<FieldType>() {
                            new FieldType() {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                api.PostTypes.Save(postTypes[0]);
                api.PostTypes.Save(postTypes[3]);
                api.PostTypes.Save(postTypes[4]);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var postTypes = api.PostTypes.GetAll();

                foreach (var p in postTypes)
                    api.PostTypes.Delete(p);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(PostTypesCached), api.IsCached);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                api.PostTypes.Save(postTypes[1]);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var models = api.PostTypes.GetAll();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.PostTypes.GetById("none-existing-type");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.PostTypes.GetById(postTypes[0].Id);

                Assert.NotNull(model);
                Assert.Equal(postTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.PostTypes.GetById(postTypes[0].Id);

                Assert.Null(model.Title);

                model.Title = "Updated";

                api.PostTypes.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.PostTypes.GetById(postTypes[3].Id);

                Assert.NotNull(model);

                api.PostTypes.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.PostTypes.GetById(postTypes[4].Id);

                Assert.NotNull(model);

                api.PostTypes.Delete(model.Id);
            }
        }
    }
}
