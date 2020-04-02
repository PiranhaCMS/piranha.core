/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Xunit;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class ParamTestsCached : ParamTests
    {
        public override Task InitializeAsync()
        {
            _cache = new Cache.SimpleCache();

            return base.InitializeAsync();
        }
    }

    [Collection("Integration tests")]
    public class ParamTests : BaseTestsAsync
    {
        private const string PARAM_1 = "MyFirstParam";
        private const string PARAM_2 = "MySecondParam";
        private const string PARAM_4 = "MyFourthParam";
        private const string PARAM_5 = "MyFifthParam";

        private readonly Guid PARAM_1_ID = Guid.NewGuid();
        private readonly string PARAM_1_VALUE = "My first value";

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                await api.Params.SaveAsync(new Param
                {
                    Id = PARAM_1_ID,
                    Key = PARAM_1,
                    Value = PARAM_1_VALUE
                });

                await api.Params.SaveAsync(new Param
                {
                    Key = PARAM_4,
                });
                await api.Params.SaveAsync(new Param
                {
                    Key = PARAM_5,
                });
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var param = await api.Params.GetAllAsync();

                foreach (var p in param)
                {
                    await api.Params.DeleteAsync(p);
                }
            }
        }

        [Fact]
        public void IsCached()
        {
            using (var api = CreateApi())
            {
                Assert.Equal(this.GetType() == typeof(ParamTestsCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public async Task Add()
        {
            using (var api = CreateApi())
            {
                await api.Params.SaveAsync(new Param
                {
                    Key = PARAM_2,
                    Value = "My second value"
                });
            }
        }

        [Fact]
        public async Task AddDuplicateKey()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () =>
                    await api.Params.SaveAsync(new Param
                    {
                        Key = PARAM_1,
                        Value = "My duplicate value"
                    })
                );
            }
        }

        [Fact]
        public async Task AddEmptyKey()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () =>
                    await api.Params.SaveAsync(new Param())
                );
            }
        }

        [Fact]
        public async Task AddTooLongKey()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () =>
                    await api.Params.SaveAsync(new Param
                    {
                        Key = "IntegerPosuereEratAnteVenenatisDapibusPosuereVelitAliquetNullamQuisRisusEgetUrnaMollisOrnareVelEuLeo",
                    })
                );
            }
        }

        [Fact]
        public async Task GetNoneById()
        {
            using (var api = CreateApi())
            {
                var none = await api.Params.GetByIdAsync(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneByKey()
        {
            using (var api = CreateApi())
            {
                var none = await api.Params.GetByKeyAsync("none-existing-key");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetAll()
        {
            using (var api = CreateApi())
            {
                var models = await api.Params.GetAllAsync();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public async Task GetById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Params.GetByIdAsync(PARAM_1_ID);

                Assert.NotNull(model);
                Assert.Equal(PARAM_1, model.Key);
            }
        }

        [Fact]
        public async Task GetByKey()
        {
            using (var api = CreateApi())
            {
                var model = await api.Params.GetByKeyAsync(PARAM_1);

                Assert.NotNull(model);
                Assert.Equal(PARAM_1, model.Key);
            }
        }

        [Fact]
        public async Task Update()
        {
            using (var api = CreateApi())
            {
                var model = await api.Params.GetByIdAsync(PARAM_1_ID);

                Assert.Equal(PARAM_1_VALUE, model.Value);

                model.Value = "Updated";

                await api.Params.SaveAsync(model);
            }
        }

        [Fact]
        public async Task Delete()
        {
            using (var api = CreateApi())
            {
                var model = await api.Params.GetByKeyAsync(PARAM_4);

                Assert.NotNull(model);

                await api.Params.DeleteAsync(model);
            }
        }

        [Fact]
        public async Task DeleteById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Params.GetByKeyAsync(PARAM_4);

                Assert.NotNull(model);

                await api.Params.DeleteAsync(model.Id);
            }
        }
    }
}
