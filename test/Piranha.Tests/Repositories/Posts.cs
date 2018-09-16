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
        private Guid SITE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();
        private Guid CAT_1_ID = Guid.NewGuid();
        private Guid POST_1_ID = Guid.NewGuid();
        private Guid POST_2_ID = Guid.NewGuid();
        private Guid POST_3_ID = Guid.NewGuid();
        private Guid POST_DI_ID = Guid.NewGuid();
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

        [Piranha.Extend.FieldType(Name = "Fourth")]
        public class MyFourthField : Extend.Fields.SimpleField<string> {
            public void Init(IMyService myService) {
                Value = myService.Value;
            }
        }        

        [PageType(Title = "Blog page")]
        public class BlogPage : Models.Page<BlogPage> { }        

        [PostType(Title = "My PostType")]
        public class MyPost : Models.Post<MyPost>
        {
            [Region]
            public TextField Ingress { get; set; }
            [Region]
            public MarkdownField Body { get; set; }
        }

        [PostType(Title = "Missing PostType")]
        public class MissingPost : Models.Post<MissingPost>
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

        [PostType(Title = "Injection PostType")]
        public class MyDIPost : Models.Post<MyDIPost>
        {
            [Region]
            public MyFourthField Body { get; set; }
        }        

        protected override void Init() {
            services = new ServiceCollection()
                .AddSingleton<IMyService, MyService>()
                .BuildServiceProvider();

            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Piranha.App.Init();

                Piranha.App.Fields.Register<MyFourthField>();                

                var pageTypeBuilder = new PageTypeBuilder(api)
                    .AddType(typeof(BlogPage));
                pageTypeBuilder.Build();
                var postTypeBuilder = new PostTypeBuilder(api)
                    .AddType(typeof(MissingPost))
                    .AddType(typeof(MyPost))
                    .AddType(typeof(MyCollectionPost))
                    .AddType(typeof(MyDIPost));
                postTypeBuilder.Build();

                // Add site
                var site = new Data.Site() {
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

                var category = new Data.Category() {
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
                post1.Blocks.Add(new Extend.Blocks.TextBlock {
                    Body = "Sollicitudin Aenean"
                });
                post1.Blocks.Add(new Extend.Blocks.TextBlock {
                    Body = "Ipsum Elit"
                });
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

                var post6 = MyDIPost.Create(api);
                post6.Id = POST_DI_ID;
                post6.BlogId = BLOG_ID;
                post6.Category = category;
                post6.Title = "My Injection Post";
                api.Posts.Save(post6);                
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                Assert.Equal(this.GetType() == typeof(PostsCached), api.IsCached);
            }
        }
        
        [Fact]
        public void GetNoneById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Posts.GetById(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Posts.GetBySlug("blog", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugId() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Posts.GetBySlug(BLOG_ID, "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugBlog() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Posts.GetBySlug("no-blog", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetNoneBySlugBlogId() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var none = api.Posts.GetBySlug(Guid.NewGuid(), "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public void GetAll() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll();

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBaseClass() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll<Models.PostBase>();

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll(BLOG_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBaseClassById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll<Models.PostBase>(BLOG_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllByIdMissing() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll<MissingPost>(BLOG_ID);

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll("blog");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBaseClassBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll<Models.PostBase>("blog");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBySlugMissing() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll<MissingPost>("blog");

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllBySlugAndSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll("blog", SITE_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllNoneById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll(Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllNoneBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll("no-blog");

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllNoneBySlugAndSite() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var posts = api.Posts.GetAll("blog", Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetGenericById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetBaseClassById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetById<Models.PostBase>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPost), model.GetType());
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", ((MyPost)model).Body.Value);
            }
        }


        [Fact]
        public void GetBlocksById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal(2, model.Blocks.Count);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[0]);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[1]);
            }            
        }        

        [Fact]
        public void GetMissingById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetById<MissingPost>(POST_1_ID);

                Assert.Null(model);
            }
        }

        [Fact]
        public void GetInfoById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetById<Models.PostInfo>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Empty(model.Blocks);
            }            
        }        

        [Fact]
        public void GetGenericBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetBySlug<MyPost>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetBaseClassBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetBySlug<Models.PostBase>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPost), model.GetType());
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", ((MyPost)model).Body.Value);
            }
        }

        [Fact]
        public void GetMissingBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetBySlug<MissingPost>("blog", "my-first-post");

                Assert.Null(model);
            }
        }

        [Fact]
        public void GetInfoBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetBySlug<Models.PostInfo>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Empty(model.Blocks);
            }
        }        

        [Fact]
        public void GetDynamicById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetById(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicBySlug() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetBySlug("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("My first post", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void CheckPermlinkSyntax() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var model = api.Posts.GetById(POST_1_ID);

                Assert.NotNull(model);
                Assert.NotNull(model.Permalink);
                Assert.StartsWith("/", model.Permalink);
            }
        }

        [Fact]
        public void GetCollectionPost() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = api.Posts.GetBySlug<MyCollectionPost>(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("Second text", post.Texts[1].Value);
            }
        }

        [Fact]
        public void GetBaseClassCollectionPost() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = api.Posts.GetBySlug<Models.PostBase>(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(typeof(MyCollectionPost), post.GetType());
                Assert.Equal(3, ((MyCollectionPost)post).Texts.Count);
                Assert.Equal("Second text", ((MyCollectionPost)post).Texts[1].Value);
            }
        }
        [Fact]
        public void GetDynamicCollectionPost() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = api.Posts.GetBySlug(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Regions.Texts.Count);
                Assert.Equal("Second text", post.Regions.Texts[1].Value);
            }
        }

        [Fact]
        public void Add() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
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
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = api.Posts.GetById<MyPost>(POST_3_ID);
                var count = api.Posts.GetAll(BLOG_ID).Count();

                Assert.NotNull(post);

                api.Posts.Delete(post);

                Assert.Equal(count - 1, api.Posts.GetAll(BLOG_ID).Count());
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var count = api.Posts.GetAll(BLOG_ID).Count();

                api.Posts.Delete(POST_2_ID);

                Assert.Equal(count - 1, api.Posts.GetAll(BLOG_ID).Count());
            }
        }

        [Fact]
        public void GetDIGeneric() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = api.Posts.GetById<MyDIPost>(POST_DI_ID);

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Body.Value);
            }            
        }

        [Fact]
        public void GetDIDynamic() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = api.Posts.GetById(POST_DI_ID);

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Regions.Body.Value);
            }            
        }

        /*
        [Fact]
        public void CreateDIGeneric() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = MyDIPost.Create(api);

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Body.Value);
            }            
        }

        [Fact]
        public void CreateDIDynamic() {
            using (var api = new Api(GetDb(), new ContentServiceFactory(services), storage, cache)) {
                var post = Models.DynamicPost.Create(api, nameof(MyDIPost));

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Regions.Body.Value);
            }            
        }
        */
    }
}
