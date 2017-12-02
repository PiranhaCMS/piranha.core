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
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static readonly Guid CAT_1_ID = Guid.NewGuid();
        public static readonly Guid POST_1_ID = Guid.NewGuid();
        public static readonly Guid POST_2_ID = Guid.NewGuid();
        public static readonly Guid POST_3_ID = Guid.NewGuid();
        protected ICache cache;
        #endregion

        [PostType(Title = "My PostType")]
        public class MyPost : Models.Post<MyPost>
        {
            [Region]
            public TextField Ingress { get; set; }
            [Region]
            public MarkdownField Body { get; set; }
        }

        [PostType(Title = "My CollectionPost")]
        public class MyCollectionPost : Models.Post<MyCollectionPost>
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

                var builder = new PostTypeBuilder(api)
                    .AddType(typeof(MyPost))
                    .AddType(typeof(MyCollectionPost));
                builder.Build();

                api.Categories.Save(new Data.Category() {
                    Id = CAT_1_ID,
                    Title = "My category",
                    ArchiveTitle = "My archive"
                });

                var post1 = MyPost.Create(api);
                post1.Id = POST_1_ID;
                post1.CategoryId = CAT_1_ID;
                post1.Title = "My first post";
                post1.Ingress = "My first ingress";
                post1.Body = "My first body";
                api.Posts.Save(post1);

                var post2 = MyPost.Create(api);
                post2.Id = POST_2_ID;
                post2.CategoryId = CAT_1_ID;
                post2.Title = "My second post";
                post2.Ingress = "My second ingress";
                post2.Body = "My second body";
                api.Posts.Save(post2);

                var post3 = MyPost.Create(api);
                post3.Id = POST_3_ID;
                post3.CategoryId = CAT_1_ID;
                post3.Title = "My third post";
                post3.Ingress = "My third ingress";
                post3.Body = "My third body";
                api.Posts.Save(post3);

                var post4 = MyCollectionPost.Create(api);
                post4.CategoryId = CAT_1_ID;
                post4.Title = "My collection post";
                post4.Texts.Add(new TextField() {
                    Value = "First text"
                });
                post4.Texts.Add(new TextField() {
                    Value = "Second text"
                });
                post4.Texts.Add(new TextField() {
                    Value = "Third text"
                });
                api.Posts.Save(post4);
            }
        }

        protected override void Cleanup() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll();
                foreach (var post in posts)
                    api.Posts.Delete(post);

                var types = api.PostTypes.GetAll();
                foreach (var t in types)
                    api.PostTypes.Delete(t);

                var category = api.Categories.GetById(CAT_1_ID);
                if (category != null)
                    api.Categories.Delete(category);
            }
        }
        
        [Fact]
        public void IsCached() {
            using (var api = new Api(GetDb(), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(PostsCached), api.IsCached);
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
                var none = api.Posts.GetBySlug("my-category", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugCategory() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var none = api.Posts.GetBySlug("no-category", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetAll();

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllByCategoryId() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetByCategoryId(CAT_1_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllByCategorySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var posts = api.Posts.GetByCategorySlug("my-category");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetGenericById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetGenericBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetBySlug<MyPost>("my-category", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetById(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicBySlug() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var model = api.Posts.GetBySlug("my-category", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("My first post", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetCollectionPost() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetBySlug<MyCollectionPost>("my-category", "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("Second text", post.Texts[1].Value);
            }
        }

        [Fact]
        public void GetDynamicCollectionPost() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetBySlug("my-category", "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Regions.Texts.Count);
                Assert.Equal("Second text", post.Regions.Texts[1].Value);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var count = api.Posts.GetAll().Count();
                var post = MyPost.Create(api, "MyPost");
                post.CategoryId = CAT_1_ID;
                post.Title = "My fourth post";
                post.Ingress = "My fourth ingress";
                post.Body = "My fourth body";

                api.Posts.Save(post);

                Assert.Equal(count + 1, api.Posts.GetAll().Count());
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
                var post = api.Posts.GetBySlug<MyCollectionPost>("my-category", "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("First text", post.Texts[0].Value);

                post.Texts[0] = "Updated text";
                post.Texts.RemoveAt(2);
                api.Posts.Save(post);

                post = api.Posts.GetBySlug<MyCollectionPost>("my-category", "my-collection-post");
                
                Assert.NotNull(post);
                Assert.Equal(2, post.Texts.Count);
                Assert.Equal("Updated text", post.Texts[0].Value);
            }
        }

        [Fact]
        public void Delete() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var post = api.Posts.GetById<MyPost>(POST_3_ID);
                var count = api.Posts.GetAll().Count();

                Assert.NotNull(post);

                api.Posts.Delete(post);

                Assert.Equal(count - 1, api.Posts.GetAll().Count());
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), storage, cache)) {
                var count = api.Posts.GetAll().Count();

                api.Posts.Delete(POST_2_ID);

                Assert.Equal(count - 1, api.Posts.GetAll().Count());
            }
        }
    }
}
