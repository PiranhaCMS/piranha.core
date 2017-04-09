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
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class PageTypes : BaseTests
    {
        #region Members
        private List<PageType> pageTypes = new List<PageType>() {
            new PageType() {
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
            new PageType() {
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
            new PageType() {
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
            new PageType() {
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
            new PageType() {
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
            using (var api = new Api(options, storage)) {
                api.PageTypes.Save(pageTypes[0]);
                api.PageTypes.Save(pageTypes[3]);
                api.PageTypes.Save(pageTypes[4]);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(options, storage)) {
                var pageTypes = api.PageTypes.GetAll();

                foreach (var p in pageTypes)
                    api.PageTypes.Delete(p);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(options, storage)) {
                api.PageTypes.Save(pageTypes[1]);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(options, storage)) {
                var models = api.PageTypes.GetAll();

                Assert.NotNull(models);
                Assert.NotEqual(0, models.Count());
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(options, storage)) {
                var none = api.PageTypes.GetById("none-existing-type");

                Assert.Null(none);
            }
        }


        [Fact]
        public void GetById() {
            using (var api = new Api(options, storage)) {
                var model = api.PageTypes.GetById(pageTypes[0].Id);

                Assert.NotNull(model);
                Assert.Equal(pageTypes[0].Regions[0].Fields[0].Id, model.Regions[0].Fields[0].Id);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(options, storage)) {
                var model = api.PageTypes.GetById(pageTypes[0].Id);

                Assert.Null(model.Title);

                model.Title = "Updated";

                api.PageTypes.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(options, storage)) {
                var model = api.PageTypes.GetById(pageTypes[3].Id);

                Assert.NotNull(model);

                api.PageTypes.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(options, storage)) {
                var model = api.PageTypes.GetById(pageTypes[4].Id);

                Assert.NotNull(model);

                api.PageTypes.Delete(model.Id);
            }
        }
    }
}
