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
    public class AliasHookTests : BaseTestsAsync
    {
        private const string ALIAS = "/alias-url";
        private readonly Guid SITE_ID = Guid.NewGuid();
        private readonly Guid ID = Guid.NewGuid();

        public class AliasOnLoadException : Exception {}
        public class AliasOnBeforeSaveException : Exception {}
        public class AliasOnAfterSaveException : Exception {}
        public class AliasOnBeforeDeleteException : Exception {}
        public class AliasOnAfterDeleteException : Exception {}

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                // Initialize
                Piranha.App.Init(api);

                // Create site
                await api.Sites.SaveAsync(new Site
                {
                    Id = SITE_ID,
                    Title = "Alias Hook Site"
                });

                // Create test alias
                await api.Aliases.SaveAsync(new Alias
                {
                    Id = ID,
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS,
                    RedirectUrl = "/redirect"
                });
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                // Remove test data
                var aliases = await api.Aliases.GetAllAsync();

                foreach (var a in aliases)
                {
                    await api.Aliases.DeleteAsync(a);
                }
                await api.Sites.DeleteAsync(SITE_ID);
            }
        }

        [Fact]
        public async Task OnLoad()
        {
            Piranha.App.Hooks.Alias.RegisterOnLoad(m => throw new AliasOnLoadException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<AliasOnLoadException>(async () =>
                {
                    await api.Aliases.GetByIdAsync(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public async Task OnBeforeSave()
        {
            Piranha.App.Hooks.Alias.RegisterOnBeforeSave(m => throw new AliasOnBeforeSaveException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<AliasOnBeforeSaveException>(async () =>
                {
                    await api.Aliases.SaveAsync(new Alias
                    {
                        SiteId = SITE_ID,
                        AliasUrl = "/my-first-alias",
                        RedirectUrl = "/my-first-redirect"
                    });
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public async Task OnAfterSave()
        {
            Piranha.App.Hooks.Alias.RegisterOnAfterSave(m => throw new AliasOnAfterSaveException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<AliasOnAfterSaveException>(async () =>
                {
                    await api.Aliases.SaveAsync(new Alias
                    {
                        SiteId = SITE_ID,
                        AliasUrl = "/my-second-alias",
                        RedirectUrl = "/my-seconf-redirect"
                    });
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public async Task OnBeforeDelete()
        {
            Piranha.App.Hooks.Alias.RegisterOnBeforeDelete(m => throw new AliasOnBeforeDeleteException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<AliasOnBeforeDeleteException>(async () =>
                {
                    await api.Aliases.DeleteAsync(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public async Task OnAfterDelete()
        {
            Piranha.App.Hooks.Alias.RegisterOnAfterDelete(m => throw new AliasOnAfterDeleteException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<AliasOnAfterDeleteException>(async () =>
                {
                    await api.Aliases.DeleteAsync(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }
    }
}
