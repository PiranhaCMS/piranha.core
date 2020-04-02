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
using System.Threading.Tasks;
using Xunit;
using Piranha.Models;

namespace Piranha.Tests.Hooks
{
    [Collection("Integration tests")]
    public class ParamHookTests : BaseTestsAsync
    {
        private const string KEY = "MyHookParam";
        private readonly Guid ID = Guid.NewGuid();

        public class ParamOnLoadException : Exception {}
        public class ParamOnBeforeSaveException : Exception {}
        public class ParamOnAfterSaveException : Exception {}
        public class ParamOnBeforeDeleteException : Exception {}
        public class ParamOnAfterDeleteException : Exception {}

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                // Initialize
                Piranha.App.Init(api);

                // Create test param
                await api.Params.SaveAsync(new Param
                {
                    Id = ID,
                    Key = KEY
                });
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                // Remove test data
                var param = await api.Params.GetAllAsync();

                foreach (var p in param)
                {
                    await api.Params.DeleteAsync(p);
                }
            }
        }

        [Fact]
        public async Task OnLoad()
        {
            Piranha.App.Hooks.Param.RegisterOnLoad(m => throw new ParamOnLoadException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ParamOnLoadException>(async () =>
                {
                    await api.Params.GetByIdAsync(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public async Task OnBeforeSave()
        {
            Piranha.App.Hooks.Param.RegisterOnBeforeSave(m => throw new ParamOnBeforeSaveException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ParamOnBeforeSaveException>(async () =>
                {
                    await api.Params.SaveAsync(new Param {
                        Key = "MyFirstHookKey"
                    });
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public async Task OnAfterSave()
        {
            Piranha.App.Hooks.Param.RegisterOnAfterSave(m => throw new ParamOnAfterSaveException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ParamOnAfterSaveException>(async () =>
                {
                    await api.Params.SaveAsync(new Param {
                        Key = "MySecondHookKey"
                    });
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public async Task OnBeforeDelete()
        {
            Piranha.App.Hooks.Param.RegisterOnBeforeDelete(m => throw new ParamOnBeforeDeleteException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ParamOnBeforeDeleteException>(async () =>
                {
                    await api.Params.DeleteAsync(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public async Task OnAfterDelete()
        {
            Piranha.App.Hooks.Param.RegisterOnAfterDelete(m => throw new ParamOnAfterDeleteException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ParamOnAfterDeleteException>(async () =>
                {
                    await api.Params.DeleteAsync(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }
    }
}
