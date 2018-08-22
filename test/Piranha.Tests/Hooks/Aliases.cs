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
    public class Aliases : BaseTests
    {
        private const string ALIAS = "/alias-url";
        private Guid SITE_ID = Guid.NewGuid();
        private Guid ID = Guid.NewGuid();

        class AliasOnLoadException : Exception {}
        class AliasOnBeforeSaveException : Exception {}
        class AliasOnAfterSaveException : Exception {}
        class AliasOnBeforeDeleteException : Exception {}
        class AliasOnAfterDeleteException : Exception {}

        protected override void Init() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Initialize
                Piranha.App.Init();

                // Create site
                api.Sites.Save(new Data.Site() {
                    Id = SITE_ID,
                    Title = "Alias Hook Site"
                });

                // Create test alias
                api.Aliases.Save(new Data.Alias() {
                    Id = ID,
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS,
                    RedirectUrl = "/redirect"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Remove test data
                var aliases = api.Aliases.GetAll();

                foreach (var a in aliases)
                    api.Aliases.Delete(a);

                api.Sites.Delete(SITE_ID);
            }
        }

        [Fact]
        public void OnLoad() {
            Piranha.App.Hooks.Alias.RegisterOnLoad(m => throw new AliasOnLoadException());

            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<AliasOnLoadException>(() => {
                    api.Aliases.GetById(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public void OnBeforeSave() {
            Piranha.App.Hooks.Alias.RegisterOnBeforeSave(m => throw new AliasOnBeforeSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<AliasOnBeforeSaveException>(() => {
                    api.Aliases.Save(new Data.Alias() {
                        SiteId = SITE_ID,
                        AliasUrl = "/my-first-alias",
                        RedirectUrl = "/my-first-redirect"
                    });
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public void OnAfterSave() {
            Piranha.App.Hooks.Alias.RegisterOnAfterSave(m => throw new AliasOnAfterSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<AliasOnAfterSaveException>(() => {
                    api.Aliases.Save(new Data.Alias() {
                        SiteId = SITE_ID,
                        AliasUrl = "/my-second-alias",
                        RedirectUrl = "/my-seconf-redirect"
                    });
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public void OnBeforeDelete() {
            Piranha.App.Hooks.Alias.RegisterOnBeforeDelete(m => throw new AliasOnBeforeDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<AliasOnBeforeDeleteException>(() => {
                    api.Aliases.Delete(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }        

        [Fact]
        public void OnAfterDelete() {
            Piranha.App.Hooks.Alias.RegisterOnAfterDelete(m => throw new AliasOnAfterDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<AliasOnAfterDeleteException>(() => {
                    api.Aliases.Delete(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();            
        }        
    }
}
