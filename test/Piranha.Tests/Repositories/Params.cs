/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.ComponentModel.DataAnnotations;
using Xunit;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class ParamsCached : Params
    {
        protected override void Init() {
            cache = new Cache.SimpleCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Params : BaseTests
    {
        #region Members
        private const string PARAM_1 = "MyFirstParam";
        private const string PARAM_2 = "MySecondParam";
        private const string PARAM_4 = "MyFourthParam";
        private const string PARAM_5 = "MyFifthParam";

        private readonly Guid PARAM_1_ID = Guid.NewGuid();
        private readonly string PARAM_1_VALUE = "My first value";
        protected ICache cache;
        #endregion

        protected override void Init() {
            using (var api = CreateApi()) {
                api.Params.Save(new Param
                {
                    Id = PARAM_1_ID,
                    Key = PARAM_1,
                    Value = PARAM_1_VALUE
                });

                api.Params.Save(new Param
                {
                    Key = PARAM_4,
                });
                api.Params.Save(new Param
                {
                    Key = PARAM_5,
                });
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var param = api.Params.GetAll();

                foreach (var p in param)
                    api.Params.Delete(p);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = CreateApi()) {
                Assert.Equal(this.GetType() == typeof(ParamsCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public void Add() {
            using (var api = CreateApi()) {
                api.Params.Save(new Param
                {
                    Key = PARAM_2,
                    Value = "My second value"
                });
            }
        }

        [Fact]
        public void AddDuplicateKey() {
            using (var api = CreateApi()) {
                Assert.Throws<ValidationException>(() =>
                    api.Params.Save(new Param
                    {
                        Key = PARAM_1,
                        Value = "My duplicate value"
                    }));
            }
        }

        [Fact]
        public void AddEmptyKey()
        {
            using (var api = CreateApi()) {
                Assert.Throws<ValidationException>(() => api.Params.Save(new Param()));
            }
        }

        [Fact]
        public void AddTooLongKey() {
            using (var api = CreateApi()) {
                Assert.Throws<ValidationException>(() =>
                    api.Params.Save(new Param
                    {
                        Key = "IntegerPosuereEratAnteVenenatisDapibusPosuereVelitAliquetNullamQuisRisusEgetUrnaMollisOrnareVelEuLeo",
                    }));
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = CreateApi()) {
                var none = api.Params.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneByKey() {
            using (var api = CreateApi()) {
                var none = api.Params.GetByKey("none-existing-key");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = CreateApi()) {
                var models = api.Params.GetAll();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = CreateApi()) {
                var model = api.Params.GetById(PARAM_1_ID);

                Assert.NotNull(model);
                Assert.Equal(PARAM_1, model.Key);
            }
        }

        [Fact]
        public void GetByKey() {
            using (var api = CreateApi()) {
                var model = api.Params.GetByKey(PARAM_1);

                Assert.NotNull(model);
                Assert.Equal(PARAM_1, model.Key);
            }
        }

        [Fact]
        public void Update() {
            using (var api = CreateApi()) {
                var model = api.Params.GetById(PARAM_1_ID);

                Assert.Equal(PARAM_1_VALUE, model.Value);

                model.Value = "Updated";

                api.Params.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = CreateApi()) {
                var model = api.Params.GetByKey(PARAM_4);

                Assert.NotNull(model);

                api.Params.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = CreateApi()) {
                var model = api.Params.GetByKey(PARAM_4);

                Assert.NotNull(model);

                api.Params.Delete(model.Id);
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
