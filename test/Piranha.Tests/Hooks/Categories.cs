/*
 * Copyright (c) 2018 Håkan Edling
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

namespace Piranha.Tests.Hooks
{
    [Collection("Integration tests")]
    public class Categories : BaseTests
    {
        private Guid SITE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();
        private Guid ID = Guid.NewGuid();

        [PageType(Title = "Blog page")]
        public class BlogPage : Models.Page<BlogPage> { }        
        class CategoryOnLoadException : Exception {}
        class CategoryOnBeforeSaveException : Exception {}
        class CategoryOnAfterSaveException : Exception {}
        class CategoryOnBeforeDeleteException : Exception {}
        class CategoryOnAfterDeleteException : Exception {}

        protected override void Init() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Initialize
                Piranha.App.Init();

                var pageTypeBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(BlogPage));
                pageTypeBuilder.Build();

                // Add site
                var site = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Hook Site",
                    IsDefault = true
                };
                api.Sites.Save(site);

                // Add blog page
                var page = BlogPage.Create(api);
                page.Id = BLOG_ID;
                page.SiteId = SITE_ID;
                page.Title = "Hook Blog";
                api.Pages.Save(page);

                // Add category
                api.Categories.Save(new Data.Category() {
                    Id = ID,
                    BlogId = BLOG_ID,
                    Title = "Hook Category"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Remove test data
                var categories = api.Categories.GetAll(BLOG_ID);
                foreach (var c in categories)
                    api.Categories.Delete(c);

                var pages = api.Pages.GetAll(SITE_ID);
                foreach (var p in pages)
                    api.Pages.Delete(p);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public void OnLoad() {
            Piranha.App.Hooks.Category.RegisterOnLoad(m => throw new CategoryOnLoadException());

            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<CategoryOnLoadException>(() => {
                    api.Categories.GetById(ID);
                });
            }
            Piranha.App.Hooks.Category.Clear();
        }

        [Fact]
        public void OnBeforeSave() {
            Piranha.App.Hooks.Category.RegisterOnBeforeSave(m => throw new CategoryOnBeforeSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<CategoryOnBeforeSaveException>(() => {
                    api.Categories.Save(new Data.Category() {
                        BlogId = BLOG_ID,
                        Title = "My First Hook Category"
                    });
                });
            }
            Piranha.App.Hooks.Category.Clear();
        }

        [Fact]
        public void OnAfterSave() {
            Piranha.App.Hooks.Category.RegisterOnAfterSave(m => throw new CategoryOnAfterSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<CategoryOnAfterSaveException>(() => {
                    api.Categories.Save(new Data.Category() {
                        BlogId = BLOG_ID,
                        Title = "My Second Hook Category"
                    });
                });
            }
            Piranha.App.Hooks.Category.Clear();
        }

        [Fact]
        public void OnBeforeDelete() {
            Piranha.App.Hooks.Category.RegisterOnBeforeDelete(m => throw new CategoryOnBeforeDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<CategoryOnBeforeDeleteException>(() => {
                    api.Categories.Delete(ID);
                });
            }
            Piranha.App.Hooks.Category.Clear();
        }        

        [Fact]
        public void OnAfterDelete() {
            Piranha.App.Hooks.Category.RegisterOnAfterDelete(m => throw new CategoryOnAfterDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<CategoryOnAfterDeleteException>(() => {
                    api.Categories.Delete(ID);
                });
            }
            Piranha.App.Hooks.Category.Clear();            
        }        
    }
}
