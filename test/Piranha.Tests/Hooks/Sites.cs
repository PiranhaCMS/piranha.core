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
    public class Sites : BaseTests
    {
        private const string TITLE = "My Hook Site";
        private readonly Guid ID = Guid.NewGuid();

        public class SiteOnLoadException : Exception {}
        public class SiteOnBeforeSaveException : Exception {}
        public class SiteOnAfterSaveException : Exception {}
        public class SiteOnBeforeDeleteException : Exception {}
        public class SiteOnAfterDeleteException : Exception {}

        protected override void Init()
        {
            using (var api = CreateApi())
            {
                // Initialize
                Piranha.App.Init(api);

                // Create test param
                api.Sites.Save(new Site
                {
                    Id = ID,
                    Title = TITLE
                });
            }
        }

        protected override void Cleanup()
        {
            using (var api = CreateApi())
            {
                // Remove test data
                var sites = api.Sites.GetAll();

                foreach (var s in sites)
                {
                    api.Sites.Delete(s);
                }
            }
        }

        [Fact]
        public void OnLoad()
        {
            Piranha.App.Hooks.Site.RegisterOnLoad(m => throw new SiteOnLoadException());
            using (var api = CreateApi())
            {
                Assert.Throws<SiteOnLoadException>(() =>
                {
                    api.Sites.GetById(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnBeforeSave()
        {
            Piranha.App.Hooks.Site.RegisterOnBeforeSave(m => throw new SiteOnBeforeSaveException());
            using (var api = CreateApi())
            {
                Assert.Throws<SiteOnBeforeSaveException>(() =>
                {
                    api.Sites.Save(new Site {
                        Title = "My First Hook Site"
                    });
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnAfterSave()
        {
            Piranha.App.Hooks.Site.RegisterOnAfterSave(m => throw new SiteOnAfterSaveException());
            using (var api = CreateApi())
            {
                Assert.Throws<SiteOnAfterSaveException>(() =>
                {
                    api.Sites.Save(new Site {
                        Title = "My Second Hook Site"
                    });
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnBeforeDelete()
        {
            Piranha.App.Hooks.Site.RegisterOnBeforeDelete(m => throw new SiteOnBeforeDeleteException());
            using (var api = CreateApi())
            {
                Assert.Throws<SiteOnBeforeDeleteException>(() =>
                {
                    api.Sites.Delete(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
        }

        [Fact]
        public void OnAfterDelete()
        {
            Piranha.App.Hooks.Site.RegisterOnAfterDelete(m => throw new SiteOnAfterDeleteException());
            using (var api = CreateApi())
            {
                Assert.Throws<SiteOnAfterDeleteException>(() =>
                {
                    api.Sites.Delete(ID);
                });
            }
            Piranha.App.Hooks.Site.Clear();
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
