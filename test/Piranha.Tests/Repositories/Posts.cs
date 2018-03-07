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
using System.Collections.Generic;
using System.Linq;
using Piranha.AttributeBuilder;
using Piranha.Data;
using Piranha.Extend.Fields;
using Piranha.Models;
using Xunit;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class PostsCached : Posts
    {
        protected override void Init() {
            cache = new Cache.MemCache();

            base.Init();
        }
    }

    [Collection("Integration tests")]
    public class Posts : BaseTests
    {
        #region Members
        private Guid SITE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();
        public Guid CAT_1_ID = Guid.NewGuid();
        public Guid POST_1_ID = Guid.NewGuid();
        public Guid POST_2_ID = Guid.NewGuid();
        public Guid POST_3_ID = Guid.NewGuid();
        protected ICache cache;
        #endregion

        [PageType(Title = "Blog page")]
        public class BlogPage : Page<BlogPage> { }        

        [PostType(Title = "My PostType")]
        public class MyPost : Post<MyPost>
        {
            [Region]
            public TextField Ingress { get; set; }
            [Region]
            public MarkdownField Body { get; set; }
        }

        [PostType(Title = "My CollectionPost")]
        public class MyCollectionPost : Post<MyCollectionPost>
        {
            [Region]
            public IList<TextField> Texts { get; set; }

            public MyCollectionPost() {
                Texts = new List<TextField>();
            }
        }

        protected override void Init() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Piranha.App.Init(api);

                var pageTypeBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(BlogPage));
                pageTypeBuilder.Build();
                var postTypeBuilder = new PostTypeBuilder(api)
                    .AddType(typeof(MyPost))
                    .AddType(typeof(MyCollectionPost));
                postTypeBuilder.Build();

                // Add site
                var site = new Site
                {
                    Id = SITE_ID,
                    Title = "Post Site",
                    InternalId = "PostSite",
                    IsDefault = true
                };
                api.Sites.Save(site);

                // Add blog page
                var page = BlogPage.Create(api);
                page.Id = BLOG_ID;
                page.SiteId = SITE_ID;
                page.Title = "Blog";
                api.Pages.Save(page);

                var category = new Category
                {
                    Id = CAT_1_ID,
                    BlogId = BLOG_ID,
                    Title = "My category"
                };
                api.Categories.Save(category);

                var post1 = MyPost.Create(api);
                post1.Id = POST_1_ID;
                post1.BlogId = BLOG_ID;
                post1.Category = category;
                post1.Title = "My first post";
                post1.Ingress = "My first ingress";
                post1.Body = "My first body";
                api.Posts.Save(post1);

                var post2 = MyPost.Create(api);
                post2.Id = POST_2_ID;
                post2.BlogId = BLOG_ID;
                post2.Category = category;
                post2.Title = "My second post";
                post2.Ingress = "My second ingress";
                post2.Body = "My second body";
                api.Posts.Save(post2);

                var post3 = MyPost.Create(api);
                post3.Id = POST_3_ID;
                post3.BlogId = BLOG_ID;
                post3.Category = category;
                post3.Title = "My third post";
                post3.Ingress = "My third ingress";
                post3.Body = "My third body";
                api.Posts.Save(post3);

                var post4 = MyCollectionPost.Create(api);
                post4.BlogId = BLOG_ID;
                post4.Category = category;
                post4.Title = "My collection post";
                post4.Texts.Add(new TextField
                {
                    Value = "First text"
                });
                post4.Texts.Add(new TextField
                {
                    Value = "Second text"
                });
                post4.Texts.Add(new TextField
                {
                    Value = "Third text"
                });
                api.Posts.Save(post4);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll(BLOG_ID);
                foreach (var post in posts)
                    api.Posts.Delete(post);

                var types = api.PostTypes.GetAll();
                foreach (var t in types)
                    api.PostTypes.Delete(t);

                var category = api.Categories.GetById(CAT_1_ID);
                if (category != null)
                    api.Categories.Delete(category);

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
        public void IsCached() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.Equal(GetType() == typeof(PostsCached), api.IsCached);
            }
        }
        
        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Posts.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Posts.GetBySlug("blog", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugId() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Posts.GetBySlug(BLOG_ID, "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugBlog() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Posts.GetBySlug("no-blog", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugBlogId() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Posts.GetBySlug(Guid.NewGuid(), "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetAllById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll(BLOG_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll("blog");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBySlugAndSite() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll("blog", SITE_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllNoneById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll(Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllNoneBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll("no-blog");

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllNoneBySlugAndSite() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll("blog", Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetGenericById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetGenericBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetBySlug<MyPost>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetById(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetBySlug("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("My first post", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void CheckPermlinkSyntax() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetById(POST_1_ID);

                Assert.NotNull(model);
                Assert.NotNull(model.Permalink);
                Assert.StartsWith("/", model.Permalink);
            }
        }

        [Fact]
        public void GetCollectionPost() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetBySlug<MyCollectionPost>(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("Second text", post.Texts[1].Value);
            }
        }

        [Fact]
        public void GetDynamicCollectionPost() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetBySlug(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Regions.Texts.Count);
                Assert.Equal("Second text", post.Regions.Texts[1].Value);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var count = api.Posts.GetAll(BLOG_ID).Count();
                var catCount = api.Categories.GetAll(BLOG_ID).Count();
                var post = MyPost.Create(api, "MyPost");
                post.BlogId = BLOG_ID;
                post.Category = "My category";
                post.Title = "My fourth post";
                post.Ingress = "My fourth ingress";
                post.Body = "My fourth body";

                api.Posts.Save(post);

                Assert.Equal(count + 1, api.Posts.GetAll(BLOG_ID).Count());
                Assert.Equal(catCount, api.Categories.GetAll(BLOG_ID).Count());
            }
        }

        [Fact]
        public void AddWithTags() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var count = api.Posts.GetAll(BLOG_ID).Count();
                var catCount = api.Categories.GetAll(BLOG_ID).Count();
                var tagCount = api.Tags.GetAll(BLOG_ID).Count();

                var post = MyPost.Create(api, "MyPost");
                post.BlogId = BLOG_ID;
                post.Category = "My category";
                post.Tags.Add("Testing", "Trying", "Adding");
                post.Title = "My fifth post";
                post.Ingress = "My fifth ingress";
                post.Body = "My fifth body";

                api.Posts.Save(post);

                Assert.Equal(count + 1, api.Posts.GetAll(BLOG_ID).Count());
                Assert.Equal(catCount, api.Categories.GetAll(BLOG_ID).Count());
                Assert.Equal(tagCount + 3, api.Tags.GetAll(BLOG_ID).Count());

                post = api.Posts.GetBySlug<MyPost>(BLOG_ID, Piranha.Utils.GenerateSlug("My fifth post"));

                Assert.NotNull(post);
                Assert.Equal(3, post.Tags.Count);
                post.Tags.Add("Another tag");

                api.Posts.Save(post);

                Assert.Equal(tagCount + 4, api.Tags.GetAll(BLOG_ID).Count());

                post = api.Posts.GetBySlug<MyPost>(BLOG_ID, Piranha.Utils.GenerateSlug("My fifth post"));

                Assert.NotNull(post);
                Assert.Equal(4, post.Tags.Count);
            }
        }

        [Fact]
        public void Update() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(post);
                Assert.Equal("My first post", post.Title);

                post.Title = "Updated post";
                api.Posts.Save(post);

                post = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(post);
                Assert.Equal("Updated post", post.Title);
            }
        }

        [Fact]
        public void UpdateCollectionPost() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetBySlug<MyCollectionPost>("blog", "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("First text", post.Texts[0].Value);

                post.Texts[0] = "Updated text";
                post.Texts.RemoveAt(2);
                api.Posts.Save(post);

                post = api.Posts.GetBySlug<MyCollectionPost>("blog", "my-collection-post");
                
                Assert.NotNull(post);
                Assert.Equal(2, post.Texts.Count);
                Assert.Equal("Updated text", post.Texts[0].Value);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetById<MyPost>(POST_3_ID);
                var count = api.Posts.GetAll(BLOG_ID).Count();

                Assert.NotNull(post);

                api.Posts.Delete(post);

                Assert.Equal(count - 1, api.Posts.GetAll(BLOG_ID).Count());
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var count = api.Posts.GetAll(BLOG_ID).Count();

                api.Posts.Delete(POST_2_ID);

                Assert.Equal(count - 1, api.Posts.GetAll(BLOG_ID).Count());
            }
        }
    }
}
