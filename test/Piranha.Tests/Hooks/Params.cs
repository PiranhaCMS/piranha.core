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
    public class Params : BaseTests
    {
        private const string KEY = "MyHookParam";
        private Guid ID = Guid.NewGuid();

        class ParamOnLoadException : Exception {}
        class ParamOnBeforeSaveException : Exception {}
        class ParamOnAfterSaveException : Exception {}
        class ParamOnBeforeDeleteException : Exception {}
        class ParamOnAfterDeleteException : Exception {}

        protected override void Init() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Initialize
                Piranha.App.Init();

                // Create test param
                api.Params.Save(new Data.Param() {
                    Id = ID,
                    Key = KEY
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Remove test data
                var param = api.Params.GetAll();

                foreach (var p in param)
                    api.Params.Delete(p);
            }
        }

        [Fact]
        public void OnLoad() {
            Piranha.App.Hooks.Param.RegisterOnLoad(m => throw new ParamOnLoadException());

            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<ParamOnLoadException>(() => {
                    api.Params.GetById(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public void OnBeforeSave() {
            Piranha.App.Hooks.Param.RegisterOnBeforeSave(m => throw new ParamOnBeforeSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<ParamOnBeforeSaveException>(() => {
                    api.Params.Save(new Data.Param() {
                        Key = "MyFirstHookKey"
                    });
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public void OnAfterSave() {
            Piranha.App.Hooks.Param.RegisterOnAfterSave(m => throw new ParamOnAfterSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<ParamOnAfterSaveException>(() => {
                    api.Params.Save(new Data.Param() {
                        Key = "MySecondHookKey"
                    });
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }

        [Fact]
        public void OnBeforeDelete() {
            Piranha.App.Hooks.Param.RegisterOnBeforeDelete(m => throw new ParamOnBeforeDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<ParamOnBeforeDeleteException>(() => {
                    api.Params.Delete(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();
        }        

        [Fact]
        public void OnAfterDelete() {
            Piranha.App.Hooks.Param.RegisterOnAfterDelete(m => throw new ParamOnAfterDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<ParamOnAfterDeleteException>(() => {
                    api.Params.Delete(ID);
                });
            }
            Piranha.App.Hooks.Param.Clear();            
        }        
    }
}
