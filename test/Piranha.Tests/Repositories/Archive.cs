/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.Extensions.DependencyInjection;
using Piranha.AttributeBuilder;
using Piranha.Extend.Fields;
using Piranha.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class ArchivesCached : Archives
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Archives : BaseTests
    {
        private Guid SITE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();

        protected ICache cache;

        public interface IMyService {
            string Value { get; }
        }

        public class MyService : IMyService {
            public string Value { get; private set; }

            public MyService() {
                Value = "My service value";
            }
        }        

        [PageType(Title = "BlogArchive")]
        public class BlogArchive : Models.ArchivePage<BlogArchive> { }

        protected override void Init() {
            services = new ServiceCollection()
                .AddSingleton<IMyService, MyService>()
                .BuildServiceProvider();

            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Piranha.App.Init();

                var pageTypeBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(BlogArchive));
                pageTypeBuilder.Build();

                // Add site
                var site = new Data.Site() {
                    Id = SITE_ID,
                    Title = "Archive Site",
                    InternalId = "ArchiveSite",
                    IsDefault = true
                };
                api.Sites.Save(site);

                var archive = BlogArchive.Create(api);
                archive.Id = BLOG_ID;
                archive.SiteId = SITE_ID;
                archive.Title = "Archive";
                api.Pages.Save(archive);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll(BLOG_ID);
                foreach (var post in posts)
                    api.Posts.Delete(post);

                var types = api.PostTypes.GetAll();
                foreach (var t in types)
                    api.PostTypes.Delete(t);

                var tags = api.Tags.GetAll(BLOG_ID);
                foreach (var tag in tags)
                    api.Tags.Delete(tag);

                api.Pages.Delete(BLOG_ID);

                var pageTypes = api.PageTypes.GetAll();
                foreach (var t in pageTypes)
                    api.PageTypes.Delete(t);

                api.Sites.Delete(SITE_ID);
            }
        }
        
        [Fact]
        public void GetAllBaseClass() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Archives.GetAll<BlogArchive>();

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBaseClassById()
        {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache))
            {
                var posts = api.Archives.GetAll<BlogArchive>(SITE_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

    }
}
