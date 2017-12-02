/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class CategoriesCached : Categories
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Categories : BaseTests
    {
        #region Members
        private const string CAT_1 = "My First Category";
        private const string CAT_2 = "My Second Category";
        private const string CAT_3 = "My Third Category";
        private const string CAT_4 = "My Fourth Category";
        private const string CAT_5 = "My Fifth Category";

        private Guid CAT_1_ID = Guid.NewGuid();
        private Guid CAT_5_ID = Guid.NewGuid();

        protected ICache cache;
        #endregion

        protected override void Init() {
            using (var api = new Api(GetDb(), storage, cache)) {
                api.Categories.Save(new Data.Category() {
                    Id = CAT_1_ID,
                    Title = CAT_1,
                    ArchiveTitle = "Archive"
                });

                api.Categories.Save(new Data.Category() {
                    Title = CAT_4,
                    ArchiveTitle = "Archive"
                });
                api.Categories.Save(new Data.Category() {
                    Id = CAT_5_ID,
                    Title = CAT_5,
                    ArchiveTitle = "Archive"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var categories = api.Categories.GetAll();

                foreach (var c in categories)
                    api.Categories.Delete(c);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(CategoriesCached), api.IsCached);
            }
        }        

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), storage, cache)) {
                api.Categories.Save(new Data.Category() {
                    Title = CAT_2,
                    ArchiveTitle = "Archive"
                });
            }
        }

        [Fact]
        public void AddDuplicateSlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.ThrowsAny<Exception>(() =>
                    api.Categories.Save(new Data.Category() {
                        Title = CAT_1,
                        ArchiveTitle = "Archive"
                    }));
            }
        }

        [Fact]
        public void AddNoTitle() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.ThrowsAny<ArgumentException>(() =>
                    api.Categories.Save(new Data.Category() {
                        ArchiveTitle = "No title"
                    }));
            }
        }

        [Fact]
        public void AddNoArchiveTitle() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.ThrowsAny<ArgumentException>(() =>
                    api.Categories.Save(new Data.Category() {
                        Title = "No archive title"
                    }));
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Categories.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Categories.GetBySlug("none-existing-slug");

                Assert.Null(none);
            }
        }


        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var models = api.Categories.GetAll();

                Assert.NotNull(models);
                Assert.NotEqual(0, models.Count());
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Categories.GetById(CAT_1_ID);

                Assert.NotNull(model);
                Assert.Equal(CAT_1, model.Title);
            }
        }

        [Fact]
        public void GetBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Categories.GetBySlug(Piranha.Utils.GenerateSlug(CAT_1));

                Assert.NotNull(model);
                Assert.Equal(CAT_1, model.Title);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Categories.GetById(CAT_1_ID);

                Assert.Equal(CAT_1, model.Title);

                model.Title = "Updated";

                api.Categories.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Categories.GetBySlug(Piranha.Utils.GenerateSlug(CAT_4));

                Assert.NotNull(model);

                api.Categories.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Categories.GetById(CAT_5_ID);

                Assert.NotNull(model);

                api.Categories.Delete(model.Id);
            }
        }
    }
}
