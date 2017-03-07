/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class Sites : BaseTests
    {
        #region Members
        private const string SITE_1 = "MyFirstSite";
        private const string SITE_2 = "MySecondSite";
        private const string SITE_3 = "MyThirdSite";
        private const string SITE_4 = "MyFourthSite";
        private const string SITE_5 = "MyFifthSite";
        private const string SITE_1_HOSTS = "mysite.com";

        private string SITE_1_ID = Guid.NewGuid().ToString();
        #endregion

        protected override void Init() {
            using (var api = new Api(options)) {
                api.Sites.Save(new Data.Site() {
                    Id = SITE_1_ID,
                    InternalId = SITE_1,
                    Title = SITE_1,
                    Hostnames = SITE_1_HOSTS,
                    IsDefault = true
                });

                api.Sites.Save(new Data.Site() {
                    InternalId = SITE_4,
                    Title = SITE_4
                });
                api.Sites.Save(new Data.Site() {
                    InternalId = SITE_5,
                    Title = SITE_5
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(options)) {
                var sites = api.Sites.GetAll();

                foreach (var site in sites)
                    api.Sites.Delete(site);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(options)) {
                api.Sites.Save(new Data.Site() {
                    InternalId = SITE_2,
                    Title = SITE_2
                });
            }
        }

        [Fact]
        public void AddDuplicateKey() {
            using (var api = new Api(options)) {
                Assert.Throws<SqlException>(() =>
                    api.Sites.Save(new Data.Site() {
                        InternalId = SITE_1,
                        Title = SITE_1
                    }));
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(options)) {
                var models = api.Sites.GetAll();

                Assert.NotNull(models);
                Assert.NotEqual(0, models.Count());
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(options)) {
                var model = api.Sites.GetById(SITE_1_ID);

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public void GetByInternalId() {
            using (var api = new Api(options)) {
                var model = api.Sites.GetByInternalId(SITE_1);

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public void GetDefault() {
            using (var api = new Api(options)) {
                var model = api.Sites.GetDefault();

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(options)) {
                var model = api.Sites.GetById(SITE_1_ID);

                Assert.Equal(SITE_1_HOSTS, model.Hostnames);

                model.Hostnames = "Updated";

                api.Sites.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(options)) {
                var model = api.Sites.GetByInternalId(SITE_4);

                Assert.NotNull(model);

                api.Sites.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(options)) {
                var model = api.Sites.GetByInternalId(SITE_5);

                Assert.NotNull(model);

                api.Sites.Delete(model.Id);
            }
        }
    }
}
