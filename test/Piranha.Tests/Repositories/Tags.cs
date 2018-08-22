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
using Piranha.Services;
using System;
using System.Data.SqlClient;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class TagsCached : Tags
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Tags : BaseTests
    {
        #region Members
        private const string TAG_1 = "My First Tag";
        private const string TAG_2 = "My Second Tag";
        private const string TAG_3 = "My Third Tag";
        private const string TAG_4 = "My Fourth Tag";
        private const string TAG_5 = "My Fifth Tag";

        private Guid SITE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();
        private Guid TAG_1_ID = Guid.NewGuid();
        private Guid TAG_5_ID = Guid.NewGuid();

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

                // Add tags
                api.Tags.Save(new Data.Tag() {
                    Id = TAG_1_ID,
                    BlogId = BLOG_ID,
                    Title = TAG_1
                });
                api.Tags.Save(new Data.Tag() {
                    BlogId = BLOG_ID,
                    Title = TAG_4
                });
                api.Tags.Save(new Data.Tag() {
                    Id = TAG_5_ID,
                    BlogId = BLOG_ID,
                    Title = TAG_5
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var tags = api.Tags.GetAll(BLOG_ID);

                foreach (var t in tags)
                    api.Tags.Delete(t);

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
                Assert.Equal(this.GetType() == typeof(TagsCached), api.IsCached);
            }
        }        

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                api.Tags.Save(new Data.Tag() {
                    BlogId = BLOG_ID,
                    Title = TAG_2
                });
            }
        }

        [Fact]
        public void AddDuplicateSlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.ThrowsAny<Exception>(() =>
                    api.Tags.Save(new Data.Tag() {
                        BlogId = BLOG_ID,
                        Title = TAG_1
                    }));
            }
        }

        [Fact]
        public void AddNoTitle() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.ThrowsAny<ArgumentException>(() =>
                    api.Tags.Save(new Data.Tag() {
                        BlogId = BLOG_ID
                    }));
            }
        }

        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Tags.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Tags.GetBySlug(BLOG_ID, "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugBlog() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Tags.GetBySlug(Guid.NewGuid(), "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var models = api.Tags.GetAll(BLOG_ID);

                Assert.NotNull(models);
                Assert.NotEmpty(models);
            }
        }

        [Fact]
        public void GetById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Tags.GetById(TAG_1_ID);

                Assert.NotNull(model);
                Assert.Equal(TAG_1, model.Title);
            }
        }

        [Fact]
        public void GetBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Tags.GetBySlug(BLOG_ID, Piranha.Utils.GenerateSlug(TAG_1));

                Assert.NotNull(model);
                Assert.Equal(TAG_1, model.Title);
            }
        }

        [Fact]
        public void GetByTitle() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Tags.GetByTitle(BLOG_ID, TAG_1);

                Assert.NotNull(model);
                Assert.Equal(TAG_1, model.Title);
            }
        }

        [Fact]
        public void GetNoneByTitle() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Tags.GetByTitle(BLOG_ID, "Missing Title");

                Assert.Null(model);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Tags.GetById(TAG_1_ID);

                Assert.Equal(TAG_1, model.Title);

                model.Title = "Updated";

                api.Tags.Save(model);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Tags.GetBySlug(BLOG_ID, Piranha.Utils.GenerateSlug(TAG_4));

                Assert.NotNull(model);

                api.Tags.Delete(model);
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Tags.GetById(TAG_5_ID);

                Assert.NotNull(model);

                api.Tags.Delete(model.Id);
            }
        }
    }
}
