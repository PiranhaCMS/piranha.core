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
    public class Params : BaseTests
    {
        #region Members
        private const string PARAM_1 = "MyFirstParam";
        private const string PARAM_2 = "MySecondParam";
        private const string PARAM_3 = "MyThirdParam";
        private const string PARAM_4 = "MyFourthParam";
        private const string PARAM_5 = "MyFifthParam";

        private string PARAM_1_ID = Guid.NewGuid().ToString();
        private string PARAM_1_VALUE = "My first value";

        private const string CUSTOM_STRING_PARAM = "CustomStringParam";
        private const string CUSTOM_BOOL_PARAM = "CustomBoolParam";
        private const string CUSTOM_OBJECT_PARAM = "CustomObjectParam";
        #endregion

        protected override void Init() {
            using (var api = new Api(options, storage)) {
                api.Params.Save(new Data.Param() {
                    Id = PARAM_1_ID,
                    Key = PARAM_1,
                    Value = PARAM_1_VALUE
                });

                api.Params.Save(new Data.Param() {
                    Key = PARAM_4,
                });
                api.Params.Save(new Data.Param() {
                    Key = PARAM_5,
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(options, storage)) {
                var param = api.Params.GetAll();

                foreach (var p in param)
                    api.Params.Delete(p);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(options, storage)) {
                api.Params.Save(new Data.Param() {
                    Key = PARAM_2,
                    Value = "My second value"
                });
            }
        }

        [Fact]
        public void AddDuplicateKey() {
            using (var api = new Api(options, storage)) {
                Assert.ThrowsAny<Exception>(() =>
                    api.Params.Save(new Data.Param() {
                        Key = PARAM_1,
                        Value = "My duplicate value"
                    }));
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(options, storage)) {
                var none = api.Params.GetById(Guid.NewGuid().ToString());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneByKey() {
            using (var api = new Api(options, storage)) {
                var none = api.Params.GetByKey("none-existing-key");

                Assert.Null(none);
            }
        }


        [Fact]
        public void GetAll() {
            using (var api = new Api(options, storage)) {
                var models = api.Params.GetAll();

                Assert.NotNull(models);
                Assert.NotEqual(0, models.Count());
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(options, storage)) {
                var model = api.Params.GetById(PARAM_1_ID);

                Assert.NotNull(model);
                Assert.Equal(PARAM_1, model.Key);
            }
        }

        [Fact]
        public void GetByKey() {
            using (var api = new Api(options, storage)) {
                var model = api.Params.GetByKey(PARAM_1);

                Assert.NotNull(model);
                Assert.Equal(PARAM_1, model.Key);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(options, storage)) {
                var model = api.Params.GetById(PARAM_1_ID);

                Assert.Equal(PARAM_1_VALUE, model.Value);

                model.Value = "Updated";

                api.Params.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(options, storage)) {
                var model = api.Params.GetByKey(PARAM_4);

                Assert.NotNull(model);

                api.Params.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(options, storage)) {
                var model = api.Params.GetByKey(PARAM_4);

                Assert.NotNull(model);

                api.Params.Delete(model.Id);
            }
        }
    }
}
