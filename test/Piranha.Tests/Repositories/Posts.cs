/*
 * Copyright (c) 2017-2020 Håkan Edling
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
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class PostsCached : Posts
    {
        public override async Task InitializeAsync()
        {
            cache = new Cache.SimpleCache();

            await base.InitializeAsync();
        }
    }

    [Collection("Integration tests")]
    public class Posts : BaseTestsAsync
    {
        private readonly Guid SITE_ID = Guid.NewGuid();
        private readonly Guid BLOG_ID = Guid.NewGuid();
        private readonly Guid CAT_1_ID = Guid.NewGuid();
        private readonly Guid POST_1_ID = Guid.NewGuid();
        private readonly Guid POST_2_ID = Guid.NewGuid();
        private readonly Guid POST_3_ID = Guid.NewGuid();
        private readonly Guid POST_DI_ID = Guid.NewGuid();
        protected ICache cache;

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
            services = new ServiceCollection()
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
                api.Sites.Save(site);

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
                Assert.Equal(this.GetType() == typeof(PostsCached), ((Api)api).IsCached);
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
        public void GetAllById() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll(BLOG_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBaseClassById() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll<Models.PostBase>(BLOG_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllByIdMissing() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll<MissingPost>(BLOG_ID);

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllBySlug() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll("blog");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBaseClassBySlug() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll<Models.PostBase>("blog");

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllBySlugMissing() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll<MissingPost>("blog");

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllBySlugAndSite() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll("blog", SITE_ID);

                Assert.NotNull(posts);
                Assert.NotEmpty(posts);
            }
        }

        [Fact]
        public void GetAllNoneById() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll(Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllNoneBySlug() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll("no-blog");

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetAllNoneBySlugAndSite() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll("blog", Guid.NewGuid());

                Assert.NotNull(posts);
                Assert.Empty(posts);
            }
        }

        [Fact]
        public void GetGenericById() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetBaseClassById() {
            using (var api = CreateApi()) {
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
            using (var api = CreateApi()) {
                var model = api.Posts.GetById<MyPost>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal(2, model.Blocks.Count);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[0]);
                Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[1]);
            }
        }

        [Fact]
        public void GetMissingById() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetById<MissingPost>(POST_1_ID);

                Assert.Null(model);
            }
        }

        [Fact]
        public void GetInfoById() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetById<Models.PostInfo>(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Empty(model.Blocks);
            }
        }

        [Fact]
        public void GetGenericBySlug() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetBySlug<MyPost>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Body.Value);
            }
        }

        [Fact]
        public void GetBaseClassBySlug() {
            using (var api = CreateApi()) {
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
            using (var api = CreateApi()) {
                var model = api.Posts.GetBySlug<MissingPost>("blog", "my-first-post");

                Assert.Null(model);
            }
        }

        [Fact]
        public void GetInfoBySlug() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetBySlug<Models.PostInfo>("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Empty(model.Blocks);
            }
        }

        [Fact]
        public void GetDynamicById() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetById(POST_1_ID);

                Assert.NotNull(model);
                Assert.Equal("my-first-post", model.Slug);
                Assert.Equal("/blog/my-first-post", model.Permalink);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void GetDynamicBySlug() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetBySlug("blog", "my-first-post");

                Assert.NotNull(model);
                Assert.Equal("My first post", model.Title);
                Assert.Equal("My first body", model.Regions.Body.Value);
            }
        }

        [Fact]
        public void CheckPermlinkSyntax() {
            using (var api = CreateApi()) {
                var model = api.Posts.GetById(POST_1_ID);

                Assert.NotNull(model);
                Assert.NotNull(model.Permalink);
                Assert.StartsWith("/", model.Permalink);
            }
        }

        [Fact]
        public void GetCollectionPost() {
            using (var api = CreateApi()) {
                var post = api.Posts.GetBySlug<MyCollectionPost>(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(3, post.Texts.Count);
                Assert.Equal("Second text", post.Texts[1].Value);
            }
        }

        [Fact]
        public void GetBaseClassCollectionPost() {
            using (var api = CreateApi()) {
                var post = api.Posts.GetBySlug<Models.PostBase>(BLOG_ID, "my-collection-post");

                Assert.NotNull(post);
                Assert.Equal(typeof(MyCollectionPost), post.GetType());
                Assert.Equal(3, ((MyCollectionPost)post).Texts.Count);
                Assert.Equal("Second text", ((MyCollectionPost)post).Texts[1].Value);
            }
        }
        [Fact]
        public void GetDynamicCollectionPost() {
            using (var api = CreateApi()) {
                var post = api.Posts.GetBySlug(BLOG_ID, "my-collection-post");

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
                var count = api.Posts.GetAll(BLOG_ID).Count();
                var catCount = api.Posts.GetAllCategories(BLOG_ID).Count();
                var post = await MyPost.CreateAsync(api, "MyPost");
                post.BlogId = BLOG_ID;
                post.Category = "My category";
                post.Title = "My fourth post";
                post.Ingress = "My fourth ingress";
                post.Body = "My fourth body";

                api.Posts.Save(post);

                Assert.Equal(count + 1, api.Posts.GetAll(BLOG_ID).Count());
                Assert.Equal(catCount, api.Posts.GetAllCategories(BLOG_ID).Count());
            }
        }

        [Fact]
        public async Task AddWithTags()
        {
            using (var api = CreateApi())
            {
                var count = api.Posts.GetAll(BLOG_ID).Count();
                var catCount = api.Posts.GetAllCategories(BLOG_ID).Count();
                var tagCount = api.Posts.GetAllTags(BLOG_ID).Count();

                var post = await MyPost.CreateAsync(api, "MyPost");
                post.BlogId = BLOG_ID;
                post.Category = "My category";
                post.Tags.Add("Testing", "Trying", "Adding");
                post.Title = "My fifth post";
                post.Ingress = "My fifth ingress";
                post.Body = "My fifth body";

                api.Posts.Save(post);

                Assert.Equal(count + 1, api.Posts.GetAll(BLOG_ID).Count());
                Assert.Equal(catCount, api.Posts.GetAllCategories(BLOG_ID).Count());
                Assert.Equal(tagCount + 3, api.Posts.GetAllTags(BLOG_ID).Count());

                post = api.Posts.GetBySlug<MyPost>(BLOG_ID, Piranha.Utils.GenerateSlug("My fifth post"));

                Assert.NotNull(post);
                Assert.Equal(3, post.Tags.Count);
                post.Tags.Add("Another tag");

                api.Posts.Save(post);

                Assert.Equal(tagCount + 4, api.Posts.GetAllTags(BLOG_ID).Count());

                post = api.Posts.GetBySlug<MyPost>(BLOG_ID, Piranha.Utils.GenerateSlug("My fifth post"));

                Assert.NotNull(post);
                Assert.Equal(4, post.Tags.Count);
            }
        }

        [Fact]
        public void Update() {
            using (var api = CreateApi()) {
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
            using (var api = CreateApi()) {
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
            using (var api = CreateApi()) {
                var post = api.Posts.GetById<MyPost>(POST_3_ID);
                var count = api.Posts.GetAll(BLOG_ID).Count();

                Assert.NotNull(post);

                api.Posts.Delete(post);

                Assert.Equal(count - 1, api.Posts.GetAll(BLOG_ID).Count());
            }
        }

        [Fact]
        public void DeleteById() {
            using (var api = CreateApi()) {
                var count = api.Posts.GetAll(BLOG_ID).Count();

                api.Posts.Delete(POST_2_ID);

                Assert.Equal(count - 1, api.Posts.GetAll(BLOG_ID).Count());
            }
        }

        [Fact]
        public void GetDIGeneric() {
            using (var api = CreateApi()) {
                var post = api.Posts.GetById<MyDIPost>(POST_DI_ID);

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Body.Value);
            }
        }

        [Fact]
        public void GetDIDynamic() {
            using (var api = CreateApi()) {
                var post = api.Posts.GetById(POST_DI_ID);

                Assert.NotNull(post);
                Assert.Equal("My service value", post.Regions.Body.Value);
            }
        }

        private IApi CreateApi()
        {
            var factory = new ContentFactory(services);
            var serviceFactory = new ContentServiceFactory(factory);

            var db = GetDb();

            return new Api(
                factory,
                new AliasRepository(db),
                new ArchiveRepository(db),
                new Piranha.Repositories.MediaRepository(db),
                new PageRepository(db, serviceFactory),
                new PageTypeRepository(db),
                new ParamRepository(db),
                new PostRepository(db, serviceFactory),
                new PostTypeRepository(db),
                new SiteRepository(db, serviceFactory),
                new SiteTypeRepository(db),
                cache: cache,
                storage: storage
            );
        }
    }
}
