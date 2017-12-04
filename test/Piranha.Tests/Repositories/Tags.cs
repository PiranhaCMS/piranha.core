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
    public class TagsCached : Tags
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Tags : BaseTests
    {
        #region Members
        private const string TAG_1 = "My First Tag";
        private const string TAG_2 = "My Second Tag";
        private const string TAG_3 = "My Third Tag";
        private const string TAG_4 = "My Fourth Tag";
        private const string TAG_5 = "My Fifth Tag";

        private Guid TAG_1_ID = Guid.NewGuid();
        private Guid TAG_5_ID = Guid.NewGuid();

        protected ICache cache;
        #endregion

        protected override void Init() {
            using (var api = new Api(GetDb(), storage, cache)) {
                api.Tags.Save(new Data.Tag() {
                    Id = TAG_1_ID,
                    Title = TAG_1
                });

                api.Tags.Save(new Data.Tag() {
                    Title = TAG_4
                });
                api.Tags.Save(new Data.Tag() {
                    Id = TAG_5_ID,
                    Title = TAG_5
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var tags = api.Tags.GetAll();

                foreach (var t in tags)
                    api.Tags.Delete(t);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(TagsCached), api.IsCached);
            }
        }        

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), storage, cache)) {
                api.Tags.Save(new Data.Tag() {
                    Title = TAG_2
                });
            }
        }

        [Fact]
        public void AddDuplicateSlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.ThrowsAny<Exception>(() =>
                    api.Tags.Save(new Data.Tag() {
                        Title = TAG_1
                    }));
            }
        }

        [Fact]
        public void AddNoTitle() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.ThrowsAny<ArgumentException>(() =>
                    api.Tags.Save(new Data.Tag()));
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Tags.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Tags.GetBySlug("none-existing-slug");

                Assert.Null(none);
            }
        }


        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var models = api.Tags.GetAll();

                Assert.NotNull(models);
                Assert.NotEqual(0, models.Count());
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Tags.GetById(TAG_1_ID);

                Assert.NotNull(model);
                Assert.Equal(TAG_1, model.Title);
            }
        }

        [Fact]
        public void GetBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Tags.GetBySlug(Piranha.Utils.GenerateSlug(TAG_1));

                Assert.NotNull(model);
                Assert.Equal(TAG_1, model.Title);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Tags.GetById(TAG_1_ID);

                Assert.Equal(TAG_1, model.Title);

                model.Title = "Updated";

                api.Tags.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Tags.GetBySlug(Piranha.Utils.GenerateSlug(TAG_4));

                Assert.NotNull(model);

                api.Tags.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Tags.GetById(TAG_5_ID);

                Assert.NotNull(model);

                api.Tags.Delete(model.Id);
            }
        }
    }
}
