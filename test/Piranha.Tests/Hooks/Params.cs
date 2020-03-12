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
    public class Params : BaseTests
    {
        private const string KEY = "MyHookParam";
        private readonly Guid ID = Guid.NewGuid();

        public class ParamOnLoadException : Exception {}
        public class ParamOnBeforeSaveException : Exception {}
        public class ParamOnAfterSaveException : Exception {}
        public class ParamOnBeforeDeleteException : Exception {}
        public class ParamOnAfterDeleteException : Exception {}

        protected override void Init()
        {
            using (var api = CreateApi())
            {
                // Initialize
                Piranha.App.Init(api);

                // Create test param
                api.Params.Save(new Param
                {
                    Id = ID,
                    Key = KEY
                });
            }
        }

        protected override void Cleanup()
        {
            using (var api = CreateApi())
            {
                // Remove test data
                var param = api.Params.GetAll();

                foreach (var p in param)
                {
                    api.Params.Delete(p);
                }
            }
        }

        [Fact]
        public void OnLoad()
        {
            Piranha.App.Hooks.Param.RegisterOnLoad(m => throw new ParamOnLoadException());
            using (var api = CreateApi())
            {
                Assert.Throws<ParamOnLoadException>(() =>
                {
                    api.Params.GetById(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public void OnBeforeSave()
        {
            Piranha.App.Hooks.Param.RegisterOnBeforeSave(m => throw new ParamOnBeforeSaveException());
            using (var api = CreateApi())
            {
                Assert.Throws<ParamOnBeforeSaveException>(() =>
                {
                    api.Params.Save(new Param {
                        Key = "MyFirstHookKey"
                    });
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public void OnAfterSave()
        {
            Piranha.App.Hooks.Param.RegisterOnAfterSave(m => throw new ParamOnAfterSaveException());
            using (var api = CreateApi())
            {
                Assert.Throws<ParamOnAfterSaveException>(() =>
                {
                    api.Params.Save(new Param {
                        Key = "MySecondHookKey"
                    });
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public void OnBeforeDelete()
        {
            Piranha.App.Hooks.Param.RegisterOnBeforeDelete(m => throw new ParamOnBeforeDeleteException());
            using (var api = CreateApi())
            {
                Assert.Throws<ParamOnBeforeDeleteException>(() =>
                {
                    api.Params.Delete(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public void OnAfterDelete()
        {
            Piranha.App.Hooks.Param.RegisterOnAfterDelete(m => throw new ParamOnAfterDeleteException());
            using (var api = CreateApi())
            {
                Assert.Throws<ParamOnAfterDeleteException>(() =>
                {
                    api.Params.Delete(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
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
