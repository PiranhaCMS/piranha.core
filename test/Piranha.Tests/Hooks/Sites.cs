/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Hooks
{
    [Collection("Integration tests")]
    public class Sites : BaseTests
    {
        private const string TITLE = "My Hook Site";
        private Guid ID = Guid.NewGuid();

        class SiteOnLoadException : Exception {}
        class SiteOnBeforeSaveException : Exception {}
        class SiteOnAfterSaveException : Exception {}
        class SiteOnBeforeDeleteException : Exception {}
        class SiteOnAfterDeleteException : Exception {}

        protected override void Init() {
            using (var api = CreateApi()) {
                // Initialize
                Piranha.App.Init(api);

                // Create test param
                api.Sites.Save(new Data.Site() {
                    Id = ID,
                    Title = TITLE
                });
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                // Remove test data
                var sites = api.Sites.GetAll();

                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public void OnLoad() {
            Piranha.App.Hooks.Site.RegisterOnLoad(m => throw new SiteOnLoadException());
            using (var api = CreateApi()) {
                Assert.Throws<SiteOnLoadException>(() => {
                    api.Sites.GetById(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnBeforeSave() {
            Piranha.App.Hooks.Site.RegisterOnBeforeSave(m => throw new SiteOnBeforeSaveException());
            using (var api = CreateApi()) {
                Assert.Throws<SiteOnBeforeSaveException>(() => {
                    api.Sites.Save(new Data.Site() {
                        Title = "My First Hook Site"
                    });
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnAfterSave() {
            Piranha.App.Hooks.Site.RegisterOnAfterSave(m => throw new SiteOnAfterSaveException());
            using (var api = CreateApi()) {
                Assert.Throws<SiteOnAfterSaveException>(() => {
                    api.Sites.Save(new Data.Site() {
                        Title = "My Second Hook Site"
                    });
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnBeforeDelete() {
            Piranha.App.Hooks.Site.RegisterOnBeforeDelete(m => throw new SiteOnBeforeDeleteException());
            using (var api = CreateApi()) {
                Assert.Throws<SiteOnBeforeDeleteException>(() => {
                    api.Sites.Delete(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnAfterDelete() {
            Piranha.App.Hooks.Site.RegisterOnAfterDelete(m => throw new SiteOnAfterDeleteException());
            using (var api = CreateApi()) {
                Assert.Throws<SiteOnAfterDeleteException>(() => {
                    api.Sites.Delete(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        private IApi CreateApi()
        {
            var factory = new ContentFactory(services);

            return new Api(GetDb(), factory, new ContentServiceFactory(factory), storage);
        }
    }
}
