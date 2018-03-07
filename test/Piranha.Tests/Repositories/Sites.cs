﻿/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Linq;
using Piranha.AttributeBuilder;
using Piranha.Data;
using Piranha.Extend.Fields;
using Piranha.Models;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class SitesCached : Sites
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Sites : BaseTests
    {
        #region Members
        private const string SITE_1 = "MyFirstSite";
        private const string SITE_2 = "MySecondSite";
        private const string SITE_3 = "MyThirdSite";
        private const string SITE_4 = "MyFourthSite";
        private const string SITE_5 = "MyFifthSite";
        private const string SITE_6 = "MySixthSite";
        private const string SITE_1_HOSTS = "mysite.com";
        protected ICache cache;

        private Guid SITE_1_ID = Guid.NewGuid();
        #endregion

        [PageType(Title = "PageType")]
        public class MyPage : Page<MyPage>
        {
            [Region]
            public TextField Text { get; set; }
        }

        protected override void Init() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Piranha.App.Init(api);

                var builder = new PageTypeBuilder(api)
                    .AddType(typeof(MyPage));
                builder.Build();

                api.Sites.Save(new Site
                {
                    Id = SITE_1_ID,
                    InternalId = SITE_1,
                    Title = SITE_1,
                    Hostnames = SITE_1_HOSTS,
                    IsDefault = true
                });

                api.Sites.Save(new Site
                {
                    InternalId = SITE_4,
                    Title = SITE_4
                });
                api.Sites.Save(new Site
                {
                    InternalId = SITE_5,
                    Title = SITE_5
                });
                api.Sites.Save(new Site
                {
                    InternalId = SITE_6,
                    Title = SITE_6
                });

                var page1 = MyPage.Create(api);
                page1.SiteId = SITE_1_ID;
                page1.Title = "Startpage";
                page1.Text = "Welcome";
                page1.IsHidden = true;
                page1.Published = DateTime.Now;
                api.Pages.Save(page1);

                var page2 = MyPage.Create(api);
                page2.SiteId = SITE_1_ID;
                page2.SortOrder = 1;
                page2.Title = "Second page";
                page2.Text = "The second page";
                api.Pages.Save(page2);

                var page3 = MyPage.Create(api);
                page3.SiteId = SITE_1_ID;
                page3.ParentId = page2.Id;
                page3.Title = "Subpage";
                page3.Text = "The subpage";
                page3.Published = DateTime.Now;
                api.Pages.Save(page3);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var pages = api.Pages.GetAll(SITE_1_ID);
                foreach (var page in pages.Where(p => p.ParentId.HasValue))
                    api.Pages.Delete(page);
                foreach (var page in pages.Where(p => !p.ParentId.HasValue))
                    api.Pages.Delete(page);

                var types = api.PageTypes.GetAll();
                foreach (var t in types)
                    api.PageTypes.Delete(t);

                var sites = api.Sites.GetAll();
                foreach (var site in sites)
                    api.Sites.Delete(site);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.Equal(GetType() == typeof(SitesCached), api.IsCached);
            }
        }        

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), storage, cache)) {
                api.Sites.Save(new Site
                {
                    InternalId = SITE_2,
                    Title = SITE_2
                });
            }
        }

        [Fact]
        public void AddDuplicateKey() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.ThrowsAny<Exception>(() =>
                    api.Sites.Save(new Site
                    {
                        InternalId = SITE_1,
                        Title = SITE_1
                    }));
            }
        }

        [Fact]
        public void AddEmptyFailure() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.ThrowsAny<ArgumentException>(() =>
                    api.Sites.Save(new Site()));
            }            
        }

        [Fact]
        public void AddAndGenerateInternalId() {
            var id = Guid.NewGuid();

            using (var api = new Api(GetDb(), storage, cache)) {
                api.Sites.Save(new Site
                {
                    Id = id,
                    Title = "Generate internal id"
                });

                var site = api.Sites.GetById(id);

                Assert.NotNull(site);
                Assert.Equal("GenerateInternalId", site.InternalId);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var models = api.Sites.GetAll();

                Assert.NotNull(models);
                Assert.NotEqual(0, models.Count());
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Sites.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneByInternalId() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Sites.GetByInternalId("none-existing-id");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Sites.GetById(SITE_1_ID);

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public void GetByInternalId() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Sites.GetByInternalId(SITE_1);

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public void GetDefault() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Sites.GetDefault();

                Assert.NotNull(model);
                Assert.Equal(SITE_1, model.InternalId);
            }
        }

        [Fact]
        public void GetSitemap() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var sitemap = api.Sites.GetSitemap();

                Assert.NotNull(sitemap);
                Assert.Equal(1, sitemap.Count);
                Assert.Equal("Startpage", sitemap[0].Title);
            }
        }

        [Fact]
        public void GetSitemapById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var sitemap = api.Sites.GetSitemap(SITE_1_ID);

                Assert.NotNull(sitemap);
                Assert.Equal(1, sitemap.Count);
                Assert.Equal("Startpage", sitemap[0].Title);
            }
        }

        [Fact]
        public void CheckPermlinkSyntax() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var sitemap = api.Sites.GetSitemap();

                foreach (var item in sitemap) {
                    Assert.NotNull(item.Permalink);
                    Assert.StartsWith("/", item.Permalink);
                }
            }
        }        

        [Fact]
        public void GetUnpublishedSitemap() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var sitemap = api.Sites.GetSitemap(onlyPublished: false);

                Assert.NotNull(sitemap);
                Assert.Equal(2, sitemap.Count);
                Assert.Equal("Startpage", sitemap[0].Title);
                Assert.Equal(1, sitemap[1].Items.Count);
                Assert.Equal("Subpage", sitemap[1].Items[0].Title);
            }
        }

        [Fact]
        public void CheckHiddenSitemapItems() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var sitemap = api.Sites.GetSitemap();

                Assert.Equal(1, sitemap.Count(s => s.IsHidden));
            }            
        }

        [Fact]
        public void ChangeDefaultSite() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var site6 = api.Sites.GetByInternalId(SITE_6);

                Assert.False(site6.IsDefault);
                site6.IsDefault = true;
                api.Sites.Save(site6);

                var site1 = api.Sites.GetById(SITE_1_ID);

                Assert.False(site1.IsDefault);
                site1.IsDefault = true;
                api.Sites.Save(site1);
            }            
        }

        [Fact]
        public void CantRemoveDefault() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var site1 = api.Sites.GetById(SITE_1_ID);

                Assert.True(site1.IsDefault);
                site1.IsDefault = false;
                api.Sites.Save(site1);

                site1 = api.Sites.GetById(SITE_1_ID);

                Assert.True(site1.IsDefault);
            }            
        }

        [Fact]
        public void GetUnpublishedSitemapById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var sitemap = api.Sites.GetSitemap(SITE_1_ID, onlyPublished: false);

                Assert.NotNull(sitemap);
                Assert.Equal(2, sitemap.Count);
                Assert.Equal("Startpage", sitemap[0].Title);
                Assert.Equal(1, sitemap[1].Items.Count);
                Assert.Equal("Subpage", sitemap[1].Items[0].Title);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Sites.GetById(SITE_1_ID);

                Assert.Equal(SITE_1_HOSTS, model.Hostnames);

                model.Hostnames = "Updated";

                api.Sites.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Sites.GetByInternalId(SITE_4);

                Assert.NotNull(model);

                api.Sites.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Sites.GetByInternalId(SITE_5);

                Assert.NotNull(model);

                api.Sites.Delete(model.Id);
            }
        }
    }
}
