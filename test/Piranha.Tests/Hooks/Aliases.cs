/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using Xunit;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Hooks
{
    [Collection("Integration tests")]
    public class Aliases : BaseTests
    {
        private const string ALIAS = "/alias-url";
        private readonly Guid SITE_ID = Guid.NewGuid();
        private readonly Guid ID = Guid.NewGuid();

        public class AliasOnLoadException : Exception {}
        public class AliasOnBeforeSaveException : Exception {}
        public class AliasOnAfterSaveException : Exception {}
        public class AliasOnBeforeDeleteException : Exception {}
        public class AliasOnAfterDeleteException : Exception {}

        protected override void Init()
        {
            using (var api = CreateApi())
            {
                // Initialize
                Piranha.App.Init(api);

                // Create site
                api.Sites.Save(new Site
                {
                    Id = SITE_ID,
                    Title = "Alias Hook Site"
                });

                // Create test alias
                api.Aliases.Save(new Alias
                {
                    Id = ID,
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS,
                    RedirectUrl = "/redirect"
                });
            }
        }

        protected override void Cleanup()
        {
            using (var api = CreateApi())
            {
                // Remove test data
                var aliases = api.Aliases.GetAll();

                foreach (var a in aliases)
                {
                    api.Aliases.Delete(a);
                }
                api.Sites.Delete(SITE_ID);
            }
        }

        [Fact]
        public void OnLoad()
        {
            Piranha.App.Hooks.Alias.RegisterOnLoad(m => throw new AliasOnLoadException());
            using (var api = CreateApi())
            {
                Assert.Throws<AliasOnLoadException>(() =>
                {
                    api.Aliases.GetById(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public void OnBeforeSave()
        {
            Piranha.App.Hooks.Alias.RegisterOnBeforeSave(m => throw new AliasOnBeforeSaveException());
            using (var api = CreateApi())
            {
                Assert.Throws<AliasOnBeforeSaveException>(() =>
                {
                    api.Aliases.Save(new Alias
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
        public void OnAfterSave()
        {
            Piranha.App.Hooks.Alias.RegisterOnAfterSave(m => throw new AliasOnAfterSaveException());
            using (var api = CreateApi())
            {
                Assert.Throws<AliasOnAfterSaveException>(() =>
                {
                    api.Aliases.Save(new Alias
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
        public void OnBeforeDelete()
        {
            Piranha.App.Hooks.Alias.RegisterOnBeforeDelete(m => throw new AliasOnBeforeDeleteException());
            using (var api = CreateApi())
            {
                Assert.Throws<AliasOnBeforeDeleteException>(() =>
                {
                    api.Aliases.Delete(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
        }

        [Fact]
        public void OnAfterDelete()
        {
            Piranha.App.Hooks.Alias.RegisterOnAfterDelete(m => throw new AliasOnAfterDeleteException());
            using (var api = CreateApi())
            {
                Assert.Throws<AliasOnAfterDeleteException>(() =>
                {
                    api.Aliases.Delete(ID);
                });
            }
            Piranha.App.Hooks.Alias.Clear();
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
                storage: storage
            );
        }
    }
}
