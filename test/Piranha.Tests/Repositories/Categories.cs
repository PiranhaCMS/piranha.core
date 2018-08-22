/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class CategoriesCached : Categories
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Categories : BaseTests
    {
        #region Members
        private const string CAT_1 = "My First Category";
        private const string CAT_2 = "My Second Category";
        private const string CAT_3 = "My Third Category";
        private const string CAT_4 = "My Fourth Category";
        private const string CAT_5 = "My Fifth Category";

        private Guid SITE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();
        private Guid CAT_1_ID = Guid.NewGuid();
        private Guid CAT_5_ID = Guid.NewGuid();

        protected ICache cache;
        #endregion

        [PageType(Title = "Blog page")]
        public class BlogPage : Models.Page<BlogPage> { }        

        protected override void Init() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Piranha.App.Init();

                var pageTypeBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(BlogPage));
                pageTypeBuilder.Build();

                // Add site
                var site = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Category Site",
                    InternalId = "CategorySite",
                    IsDefault = true
                };
                api.Sites.Save(site);

                // Add blog page
                var page = BlogPage.Create(api);
                page.Id = BLOG_ID;
                page.SiteId = SITE_ID;
                page.Title = "Blog";
                api.Pages.Save(page);

                // Add categories
                api.Categories.Save(new Data.Category() {
                    Id = CAT_1_ID,
                    BlogId = BLOG_ID,
                    Title = CAT_1
                });

                api.Categories.Save(new Data.Category() {
                    BlogId = BLOG_ID,
                    Title = CAT_4
                });
                api.Categories.Save(new Data.Category() {
                    Id = CAT_5_ID,
                    BlogId = BLOG_ID,
                    Title = CAT_5
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var categories = api.Categories.GetAll(BLOG_ID);

                foreach (var c in categories)
                    api.Categories.Delete(c);

                api.Pages.Delete(BLOG_ID);

                var types = api.PageTypes.GetAll();
                foreach (var t in types)
                    api.PageTypes.Delete(t);

                api.Sites.Delete(SITE_ID);
            }
        }

        [Fact]
        public void IsCached() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(CategoriesCached), api.IsCached);
            }
        }        

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                api.Categories.Save(new Data.Category() {
                    BlogId = BLOG_ID,
                    Title = CAT_2
                });
            }
        }

        [Fact]
        public void AddDuplicateSlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.ThrowsAny<Exception>(() =>
                    api.Categories.Save(new Data.Category() {
                        BlogId = BLOG_ID,
                        Title = CAT_1
                    }));
            }
        }

        [Fact]
        public void AddNoTitle() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.ThrowsAny<ArgumentException>(() =>
                    api.Categories.Save(new Data.Category() {
                        BlogId = BLOG_ID
                    }));
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Categories.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Categories.GetBySlug(BLOG_ID, "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var models = api.Categories.GetAll(BLOG_ID);

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetNone() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var models = api.Categories.GetAll(Guid.NewGuid());

                Assert.NotNull(models);
                Assert.Empty(models);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Categories.GetById(CAT_1_ID);

                Assert.NotNull(model);
                Assert.Equal(CAT_1, model.Title);
            }
        }

        [Fact]
        public void GetBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Categories.GetBySlug(BLOG_ID, Piranha.Utils.GenerateSlug(CAT_1));

                Assert.NotNull(model);
                Assert.Equal(CAT_1, model.Title);
            }
        }

        [Fact]
        public void GetByTitle() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Categories.GetByTitle(BLOG_ID, CAT_1);

                Assert.NotNull(model);
                Assert.Equal(CAT_1, model.Title);
            }
        }

        [Fact]
        public void GetNoneByTitle() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Categories.GetByTitle(BLOG_ID, "Missing Title");

                Assert.Null(model);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Categories.GetById(CAT_1_ID);

                Assert.Equal(CAT_1, model.Title);

                model.Title = "Updated";

                api.Categories.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Categories.GetBySlug(BLOG_ID, Piranha.Utils.GenerateSlug(CAT_4));

                Assert.NotNull(model);

                api.Categories.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Categories.GetById(CAT_5_ID);

                Assert.NotNull(model);

                api.Categories.Delete(model.Id);
            }
        }
    }
}
