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
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class AliasesCached : Aliases
    {
        protected override void Init() {
            cache = new Cache.SimpleCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Aliases : BaseTests
    {
        #region Members
        private const string ALIAS_1 = "/old-url";
        private const string ALIAS_2 = "/another-old-url";
        private const string ALIAS_3 = "/moved/page";
        private const string ALIAS_4 = "/another-moved-page";
        private const string ALIAS_5 = "/the-last-moved-page";

        private Guid SITE_ID = Guid.NewGuid();
        private Guid ALIAS_1_ID = Guid.NewGuid();
        protected ICache cache;
        #endregion

        protected override void Init() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                // Add site
                var site = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Alias Site",
                    InternalId = "AliasSite",
                    IsDefault = true
                };
                api.Sites.Save(site);

                // Add aliases
                api.Aliases.Save(new Data.Alias() {
                    Id = ALIAS_1_ID,
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_1,
                    RedirectUrl = "/redirect-1"
                });

                api.Aliases.Save(new Data.Alias() {
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_4,
                    RedirectUrl = "/redirect-4"
                });
                api.Aliases.Save(new Data.Alias() {
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_5,
                    RedirectUrl = "/redirect-5"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var aliases = api.Aliases.GetAll();
                foreach (var a in aliases)
                    api.Aliases.Delete(a);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(AliasesCached), api.IsCached);
            }
        }        

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                api.Aliases.Save(new Data.Alias() {
                    SiteId = SITE_ID,
                    AliasUrl = ALIAS_2,
                    RedirectUrl = "/redirect-2"
                });
            }
        }

        [Fact]
        public void AddDuplicateKey() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.ThrowsAny<Exception>(() =>
                    api.Aliases.Save(new Data.Alias() {
                        SiteId = SITE_ID,
                        AliasUrl = ALIAS_1,
                        RedirectUrl = "/duplicate-alias"
                    }));
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Aliases.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneByAliasUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Aliases.GetByAliasUrl("/none-existing-alias");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneByRedirectUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Aliases.GetByRedirectUrl("/none-existing-alias");

                Assert.Empty(none);
            }            
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var models = api.Aliases.GetAll();

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Aliases.GetById(ALIAS_1_ID);

                Assert.NotNull(model);
                Assert.Equal(ALIAS_1, model.AliasUrl);
            }
        }

        [Fact]
        public void GetByAliasUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Aliases.GetByAliasUrl(ALIAS_1);

                Assert.NotNull(model);
                Assert.Equal(ALIAS_1, model.AliasUrl);
            }
        }

        [Fact]
        public void GetByRedirectUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var models = api.Aliases.GetByRedirectUrl("/redirect-1");

                Assert.Single(models);
                Assert.Equal(ALIAS_1, models.First().AliasUrl);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Aliases.GetById(ALIAS_1_ID);

                Assert.Equal("/redirect-1", model.RedirectUrl);

                model.RedirectUrl = "/redirect-updated";

                api.Aliases.Save(model);
            }
        }

        [Fact]
        public void FixAliasUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = new Data.Alias() {
                    SiteId = SITE_ID,
                    AliasUrl = "the-alias-url-1",
                    RedirectUrl = "/the-redirect-1"
                };

                api.Aliases.Save(model);

                Assert.Equal("/the-alias-url-1", model.AliasUrl);
            }            
        }

        [Fact]
        public void FixRedirectUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = new Data.Alias() {
                    SiteId = SITE_ID,
                    AliasUrl = "/the-alias-url-2",
                    RedirectUrl = "the-redirect-2"
                };

                api.Aliases.Save(model);

                Assert.Equal("/the-redirect-2", model.RedirectUrl);
            }            
        }

        [Fact]
        public void AllowHttpUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = new Data.Alias() {
                    SiteId = SITE_ID,
                    AliasUrl = "/the-alias-url-3",
                    RedirectUrl = "http://redirect.com"
                };

                api.Aliases.Save(model);

                Assert.Equal("http://redirect.com", model.RedirectUrl);
            }                        
        }

        [Fact]
        public void AllowHttpsUrl() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = new Data.Alias() {
                    SiteId = SITE_ID,
                    AliasUrl = "/the-alias-url-4",
                    RedirectUrl = "https://redirect.com"
                };

                api.Aliases.Save(model);

                Assert.Equal("https://redirect.com", model.RedirectUrl);
            }                        
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Aliases.GetByAliasUrl(ALIAS_4);

                Assert.NotNull(model);

                api.Aliases.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Aliases.GetByAliasUrl(ALIAS_5);

                Assert.NotNull(model);

                api.Aliases.Delete(model.Id);
            }
        }
    }
}
