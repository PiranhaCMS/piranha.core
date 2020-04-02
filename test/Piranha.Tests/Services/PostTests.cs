/*
 * Copyright (c) .NET Foundation and Contributors
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
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Piranha.AttributeBuilder;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;

namespace Piranha.Tests.Services
{
    [Collection("Integration tests")]
    public class PostTestsCached : PostTests
    {
        public override async Task InitializeAsync()
        {
            _cache = new Cache.SimpleCache();

            await base.InitializeAsync();
        }
    }

    [Collection("Integration tests")]
    public class PostTests : BaseTestsAsync
    {
        private readonly Guid SITE_ID = Guid.NewGuid();
        private readonly Guid BLOG_ID = Guid.NewGuid();
        private readonly Guid CAT_1_ID = Guid.NewGuid();
        private readonly Guid POST_1_ID = Guid.NewGuid();
        private readonly Guid POST_2_ID = Guid.NewGuid();
        private readonly Guid POST_3_ID = Guid.NewGuid();
        private readonly Guid POST_DI_ID = Guid.NewGuid();

        public interface IMyService
        {
            string Value { get; }
        }

        public class MyService : IMyService
        {
            public string Value { get; private set; } = "My service value";
        }

        [Piranha.Extend.FieldType(Name = "Fourth")]
        public class MyFourthField : Extend.Fields.SimpleField<string>
        {
            public void Init(IMyService myService)
            {
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
            public IList<TextField> Texts { get; set; } = new List<TextField>();
        }

        [PostType(Title = "Injection PostType")]
        public class MyDIPost : Models.Post<MyDIPost>
        {
            [Region]
            public MyFourthField Body { get; set; }
        }

        public override async Task InitializeAsync()
        {
            _services = new ServiceCollection()
                .AddSingleton<IMyService, MyService>()
                .BuildServiceProvider();

            using (var api = CreateApi())
            {
                Piranha.App.Init(api);

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
                var site = new Site
                {
                    Id = SITE_ID,
                    Title = "Post Site",
                    InternalId = "PostSite",
                    IsDefault = true
                };
                await api.Sites.SaveAsync(site);

                // Add blog page
                var page = await BlogPage.CreateAsync(api);
                page.Id = BLOG_ID;
                page.SiteId = SITE_ID;
                page.Title = "Blog";
                await api.Pages.SaveAsync(page);

                var category = new Models.Taxonomy
                {
                    Id = CAT_1_ID,
                    Title = "My category"
                };

                var post1 = await MyPost.CreateAsync(api);
                post1.Id = POST_1_ID;
                post1.BlogId = BLOG_ID;
                post1.Category = category;
                post1.Title = "My first post";
                post1.Ingress = "My first ingress";
                post1.Body = "My first body";
                post1.Blocks.Add(new Extend.Blocks.TextBlock
                {
                    Body = "Sollicitudin Aenean"
                });
                post1.Blocks.Add(new Extend.Blocks.TextBlock
                {
                    Body = "Ipsum Elit"
                });
                await api.Posts.SaveAsync(post1);

                var post2 = await MyPost.CreateAsync(api);
                post2.Id = POST_2_ID;
                post2.BlogId = BLOG_ID;
                post2.Category = category;
                post2.Title = "My second post";
                post2.Ingress = "My second ingress";
                post2.Body = "My second body";
                await api.Posts.SaveAsync(post2);

                var post3 = await MyPost.CreateAsync(api);
                post3.Id = POST_3_ID;
                post3.BlogId = BLOG_ID;
                post3.Category = category;
                post3.Title = "My third post";
                post3.Ingress = "My third ingress";
                post3.Body = "My third body";
                await api.Posts.SaveAsync(post3);

                var post4 = await MyCollectionPost.CreateAsync(api);
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
                await api.Posts.SaveAsync(post4);

                var post6 = await MyDIPost.CreateAsync(api);
                post6.Id = POST_DI_ID;
                post6.BlogId = BLOG_ID;
                post6.Category = category;
                post6.Title = "My Injection Post";
                await api.Posts.SaveAsync(post6);
            }
        }

        public override async Task DisposeAsync()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync(BLOG_ID);
                foreach (var post in posts)
                {
                    await api.Posts.DeleteAsync(post);
                }

                var types = await api.PostTypes.GetAllAsync();
                foreach (var t in types)
                {
                    await api.PostTypes.DeleteAsync(t);
                }

                await api.Pages.DeleteAsync(BLOG_ID);

                var pageTypes = await api.PageTypes.GetAllAsync();
                foreach (var t in pageTypes)
                {
                    await api.PageTypes.DeleteAsync(t);
                }

                await api.Sites.DeleteAsync(SITE_ID);
            }
        }

        [Fact]
        public void IsCached()
        {
            using (var api = CreateApi())
            {
                Assert.Equal(this.GetType() == typeof(PostTestsCached), ((Api)api).IsCached);
            }
        }

        [Fact]
        public async Task GetNoneById()
        {
            using (var api = CreateApi())
            {
                var none = await api.Posts.GetByIdAsync(Guid.NewGuid());

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneBySlug()
        {
            using (var api = CreateApi())
            {
                var none = await api.Posts.GetBySlugAsync("blog", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneBySlugId()
        {
            using (var api = CreateApi())
            {
                var none = await api.Posts.GetBySlugAsync(BLOG_ID, "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneBySlugBlog()
        {
            using (var api = CreateApi())
            {
                var none = await api.Posts.GetBySlugAsync("no-blog", "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetNoneBySlugBlogId()
        {
            using (var api = CreateApi())
            {
                var none = await api.Posts.GetBySlugAsync(Guid.NewGuid(), "none-existing-slug");

                Assert.Null(none);
            }
        }

        [Fact]
        public async Task GetAll()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllBySiteIdAsync();

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetAllBaseClass()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllBySiteIdAsync<Models.PostBase>();

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetAllById()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync(BLOG_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetAllBaseClassById()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync<Models.PostBase>(BLOG_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetAllByIdMissing()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync<MissingPost>(BLOG_ID);

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public async Task GetAllBySlug()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync("blog");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetAllBaseClassBySlug()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync<Models.PostBase>("blog");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetAllBySlugMissing()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync<MissingPost>("blog");

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public async Task GetAllBySlugAndSite()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync("blog", SITE_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public async Task GetAllNoneById()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync(Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public async Task GetAllNoneBySlug()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync("no-blog");

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public async Task GetAllNoneBySlugAndSite()
        {
            using (var api = CreateApi())
            {
                var posts = await api.Posts.GetAllAsync("blog", Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public async Task GetGenericById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetByIdAsync<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public async Task GetBaseClassById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetByIdAsync<Models.PostBase>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPost), model.GetType());
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", ((MyPost)model).Body.Value);
            }
        }

        [Fact]
        public async Task GetBlocksById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetByIdAsync<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal(2, model.Blocks.Count);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[0]);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[1]);
            }
        }

        [Fact]
        public async Task GetMissingById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetByIdAsync<MissingPost>(POST_1_ID);

                Assert.Null(model);
            }
        }

        [Fact]
        public async Task GetInfoById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetByIdAsync<Models.PostInfo>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Empty(model.Blocks);
            }
        }

        [Fact]
        public async Task GetGenericBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetBySlugAsync<MyPost>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public async Task GetBaseClassBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetBySlugAsync<Models.PostBase>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal(typeof(MyPost), model.GetType());
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", ((MyPost)model).Body.Value);
            }
        }

        [Fact]
        public async Task GetMissingBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetBySlugAsync<MissingPost>("blog", "my-first-post");

                Assert.Null(model);
            }
        }

        [Fact]
        public async Task GetInfoBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetBySlugAsync<Models.PostInfo>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Empty(model.Blocks);
            }
        }

        [Fact]
        public async Task GetDynamicById()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetByIdAsync(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public async Task GetDynamicBySlug()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetBySlugAsync("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("My first post", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public async Task CheckPermlinkSyntax()
        {
            using (var api = CreateApi())
            {
                var model = await api.Posts.GetByIdAsync(POST_1_ID);

                Assert.NotNull(model);
                Assert.NotNull(model.Permalink);
                Assert.StartsWith("/", model.Permalink);
            }
        }

        [Fact]
        public async Task GetCollectionPost()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetBySlugAsync<MyCollectionPost>(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("Second text", post.Texts[1].Value);
            }
        }

        [Fact]
        public async Task GetBaseClassCollectionPost()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetBySlugAsync<Models.PostBase>(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(typeof(MyCollectionPost), post.GetType());
                Assert.Equal(3, ((MyCollectionPost)post).Texts.Count);
                Assert.Equal("Second text", ((MyCollectionPost)post).Texts[1].Value);
            }
        }
        [Fact]
        public async Task GetDynamicCollectionPost()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetBySlugAsync(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Regions.Texts.Count);
                Assert.Equal("Second text", post.Regions.Texts[1].Value);
            }
        }

        [Fact]
        public async Task Add()
        {
            using (var api = CreateApi())
            {
                var count = (await api.Posts.GetAllAsync(BLOG_ID)).Count();
                var catCount = (await api.Posts.GetAllCategoriesAsync(BLOG_ID)).Count();
                var post = await MyPost.CreateAsync(api, "MyPost");
                post.BlogId = BLOG_ID;
                post.Category = "My category";
                post.Title = "My fourth post";
                post.Ingress = "My fourth ingress";
                post.Body = "My fourth body";

                await api.Posts.SaveAsync(post);

                Assert.Equal(count + 1, (await api.Posts.GetAllAsync(BLOG_ID)).Count());
                Assert.Equal(catCount, (await api.Posts.GetAllCategoriesAsync(BLOG_ID)).Count());
            }
        }

        [Fact]
        public async Task AddWithTags()
        {
            using (var api = CreateApi())
            {
                var count = (await api.Posts.GetAllAsync(BLOG_ID)).Count();
                var catCount = (await api.Posts.GetAllCategoriesAsync(BLOG_ID)).Count();
                var tagCount = (await api.Posts.GetAllTagsAsync(BLOG_ID)).Count();

                var post = await MyPost.CreateAsync(api, "MyPost");
                post.BlogId = BLOG_ID;
                post.Category = "My category";
                post.Tags.Add("Testing", "Trying", "Adding");
                post.Title = "My fifth post";
                post.Ingress = "My fifth ingress";
                post.Body = "My fifth body";

                await api.Posts.SaveAsync(post);

                Assert.Equal(count + 1, (await api.Posts.GetAllAsync(BLOG_ID)).Count());
                Assert.Equal(catCount, (await api.Posts.GetAllCategoriesAsync(BLOG_ID)).Count());
                Assert.Equal(tagCount + 3, (await api.Posts.GetAllTagsAsync(BLOG_ID)).Count());

                post = await api.Posts.GetBySlugAsync<MyPost>(BLOG_ID, Piranha.Utils.GenerateSlug("My fifth post"));

                Assert.NotNull(post);
                Assert.Equal(3, post.Tags.Count);
                post.Tags.Add("Another tag");

                await api.Posts.SaveAsync(post);

                Assert.Equal(tagCount + 4, (await api.Posts.GetAllTagsAsync(BLOG_ID)).Count());

                post = await api.Posts.GetBySlugAsync<MyPost>(BLOG_ID, Piranha.Utils.GenerateSlug("My fifth post"));

                Assert.NotNull(post);
                Assert.Equal(4, post.Tags.Count);
            }
        }

        [Fact]
        public async Task Update()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetByIdAsync<MyPost>(POST_1_ID);

                Assert.NotNull(post);
                Assert.Equal("My first post", post.Title);

                post.Title = "Updated post";
                await api.Posts.SaveAsync(post);

                post = await api.Posts.GetByIdAsync<MyPost>(POST_1_ID);

                Assert.NotNull(post);
                Assert.Equal("Updated post", post.Title);
            }
        }

        [Fact]
        public async Task UpdateCollectionPost()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetBySlugAsync<MyCollectionPost>("blog", "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("First text", post.Texts[0].Value);

                post.Texts[0] = "Updated text";
                post.Texts.RemoveAt(2);
                await api.Posts.SaveAsync(post);

                post = await api.Posts.GetBySlugAsync<MyCollectionPost>("blog", "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(2, post.Texts.Count);
                Assert.Equal("Updated text", post.Texts[0].Value);
            }
        }

        [Fact]
        public async Task Delete()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetByIdAsync<MyPost>(POST_3_ID);
                var count = (await api.Posts.GetAllAsync(BLOG_ID)).Count();

                Assert.NotNull(post);

                await api.Posts.DeleteAsync(post);

                Assert.Equal(count - 1, (await api.Posts.GetAllAsync(BLOG_ID)).Count());
            }
        }

        [Fact]
        public async Task DeleteById()
        {
            using (var api = CreateApi())
            {
                var count = (await api.Posts.GetAllAsync(BLOG_ID)).Count();

                await api.Posts.DeleteAsync(POST_2_ID);

                Assert.Equal(count - 1, (await api.Posts.GetAllAsync(BLOG_ID)).Count());
            }
        }

        [Fact]
        public async Task GetDIGeneric()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetByIdAsync<MyDIPost>(POST_DI_ID);

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Body.Value);
            }
        }

        [Fact]
        public async Task GetDIDynamic()
        {
            using (var api = CreateApi())
            {
                var post = await api.Posts.GetByIdAsync(POST_DI_ID);

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Regions.Body.Value);
            }
        }
    }
}
