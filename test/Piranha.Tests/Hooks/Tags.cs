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
    public class Tags : BaseTests
    {
        private Guid SITE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();
        private Guid ID = Guid.NewGuid();

        [PageType(Title = "Blog page")]
        public class BlogPage : Models.Page<BlogPage> { }        
        class TagOnLoadException : Exception {}
        class TagOnBeforeSaveException : Exception {}
        class TagOnAfterSaveException : Exception {}
        class TagOnBeforeDeleteException : Exception {}
        class TagOnAfterDeleteException : Exception {}

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

                // Add tag
                api.Tags.Save(new Data.Tag() {
                    Id = ID,
                    BlogId = BLOG_ID,
                    Title = "Hook Tag"
                });
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                // Remove test data
                var tags = api.Tags.GetAll(BLOG_ID);
                foreach (var t in tags)
                    api.Tags.Delete(t);

                var pages = api.Pages.GetAll(SITE_ID);
                foreach (var p in pages)
                    api.Pages.Delete(p);

                var types = api.PageTypes.GetAll();
                foreach (var t in types)
                    api.PageTypes.Delete(t);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public void OnLoad() {
            Piranha.App.Hooks.Tag.RegisterOnLoad(m => throw new TagOnLoadException());

            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<TagOnLoadException>(() => {
                    api.Tags.GetById(ID);
                });
            }
            Piranha.App.Hooks.Tag.Clear();
        }

        [Fact]
        public void OnBeforeSave() {
            Piranha.App.Hooks.Tag.RegisterOnBeforeSave(m => throw new TagOnBeforeSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<TagOnBeforeSaveException>(() => {
                    api.Tags.Save(new Data.Tag() {
                        BlogId = BLOG_ID,
                        Title = "My First Hook Tag"
                    });
                });
            }
            Piranha.App.Hooks.Tag.Clear();
        }

        [Fact]
        public void OnAfterSave() {
            Piranha.App.Hooks.Tag.RegisterOnAfterSave(m => throw new TagOnAfterSaveException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<TagOnAfterSaveException>(() => {
                    api.Tags.Save(new Data.Tag() {
                        BlogId = BLOG_ID,
                        Title = "My Second Hook Tag"
                    });
                });
            }
            Piranha.App.Hooks.Tag.Clear();
        }

        [Fact]
        public void OnBeforeDelete() {
            Piranha.App.Hooks.Tag.RegisterOnBeforeDelete(m => throw new TagOnBeforeDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<TagOnBeforeDeleteException>(() => {
                    api.Tags.Delete(ID);
                });
            }
            Piranha.App.Hooks.Tag.Clear();
        }        

        [Fact]
        public void OnAfterDelete() {
            Piranha.App.Hooks.Tag.RegisterOnAfterDelete(m => throw new TagOnAfterDeleteException());
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage)) {
                Assert.Throws<TagOnAfterDeleteException>(() => {
                    api.Tags.Delete(ID);
                });
            }
            Piranha.App.Hooks.Tag.Clear();            
        }        
    }
}
