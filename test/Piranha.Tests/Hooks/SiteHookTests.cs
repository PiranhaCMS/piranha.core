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
    public class SiteHookTests : BaseTestsAsync
    {
        private const string TITLE = "My Hook Site";
        private readonly Guid ID = Guid.NewGuid();

        public class SiteOnLoadException : Exception {}
        public class SiteOnBeforeSaveException : Exception {}
        public class SiteOnAfterSaveException : Exception {}
        public class SiteOnBeforeDeleteException : Exception {}
        public class SiteOnAfterDeleteException : Exception {}

        public override async Task InitializeAsync()
        {
            using (var api = CreateApi())
            {
                // Initialize
                Piranha.App.Init(api);

                // Create test param
                await api.Sites.SaveAsync(new Site
                {
                    Id = ID,
                    Title = TITLE
                });
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                // Remove test data
                var sites = await api.Sites.GetAllAsync();

                foreach (var s in sites)
                {
                    await api.Sites.DeleteAsync(s);
                }
            }
        }

        [Fact]
        public async Task OnLoad()
        {
            Piranha.App.Hooks.Site.RegisterOnLoad(m => throw new SiteOnLoadException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<SiteOnLoadException>(async () =>
                {
                    await api.Sites.GetByIdAsync(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public async Task OnBeforeSave()
        {
            Piranha.App.Hooks.Site.RegisterOnBeforeSave(m => throw new SiteOnBeforeSaveException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<SiteOnBeforeSaveException>(async () =>
                {
                    await api.Sites.SaveAsync(new Site {
                        Title = "My First Hook Site"
                    });
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public async Task OnAfterSave()
        {
            Piranha.App.Hooks.Site.RegisterOnAfterSave(m => throw new SiteOnAfterSaveException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<SiteOnAfterSaveException>(async () =>
                {
                    await api.Sites.SaveAsync(new Site {
                        Title = "My Second Hook Site"
                    });
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public async Task OnBeforeDelete()
        {
            Piranha.App.Hooks.Site.RegisterOnBeforeDelete(m => throw new SiteOnBeforeDeleteException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<SiteOnBeforeDeleteException>(async () =>
                {
                    await api.Sites.DeleteAsync(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public async Task OnAfterDelete()
        {
            Piranha.App.Hooks.Site.RegisterOnAfterDelete(m => throw new SiteOnAfterDeleteException());
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<SiteOnAfterDeleteException>(async () =>
                {
                    await api.Sites.DeleteAsync(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }
    }
}
