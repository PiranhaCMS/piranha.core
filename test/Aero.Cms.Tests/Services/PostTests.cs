using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Aero.Cms.AttributeBuilder;
using Aero.Cms.Extend;
using Aero.Cms.Extend.Fields;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

//[Collection("Integration tests")]
public class PostTestsMemoryCache(MartenFixture fixture) : PostTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

//[Collection("Integration tests")]
public class PostTestsDistributedCache(MartenFixture fixture) : PostTests(fixture)
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

//[Collection("Integration tests")]
public class PostTests(MartenFixture fixture) : AsyncTestBase(fixture)
{
    private readonly string SITEID = Snowflake.NewId();
    private readonly string BLOGID = Snowflake.NewId();
    private readonly string POST1ID = Snowflake.NewId();
    private readonly string POST2ID = Snowflake.NewId();
    private readonly string POST3ID = Snowflake.NewId();
    private readonly string POSTDIID = Snowflake.NewId();

    public class MyService : IMyService
    {
        public string Value { get; private set; } = "My service value";
    }

    [Aero.Cms.Extend.FieldType(Name = "Fourth")]
    public class MyFourthField : Extend.Fields.SimpleField<string>
    {
        public void Init(IMyService myService)
        {
            Value = myService.Value;
        }
    }

    [PageType(Title = "Blog page")]
    public class BlogPage : Models.Page<BlogPage>
    {
    }

    [PostType(Title = "My PostType")]
    public class MyPost : Models.Post<MyPost>
    {
        [Region] public TextField Ingress { get; set; }
        [Region] public MarkdownField Body { get; set; }
    }

    [PostType(Title = "Missing PostType")]
    public class MissingPost : Models.Post<MissingPost>
    {
        [Region] public TextField Ingress { get; set; }
        [Region] public MarkdownField Body { get; set; }
    }

    [PostType(Title = "My CollectionPost")]
    public class MyCollectionPost : Models.Post<MyCollectionPost>
    {
        [Region] public List<TextField> Texts { get; set; } = new List<TextField>();
    }

    [PostType(Title = "Injection PostType")]
    public class MyDIPost : Models.Post<MyDIPost>
    {
        [Region] public MyFourthField Body { get; set; }
    }

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        Aero.Cms.App.Fields.Register<MyFourthField>();

        new ContentTypeBuilder(api)
            .AddType(typeof(BlogPage))
            .AddType(typeof(MissingPost))
            .AddType(typeof(MyPost))
            .AddType(typeof(MyCollectionPost))
            .AddType(typeof(MyDIPost))
            .Build();

        await AddSampleData();
    }

    protected async Task AddSampleData()
    {
        // Add site
        var site = new Site
        {
            Id = SITEID,
            Title = "Post Site",
            InternalId = "PostSite",
            IsDefault = true
        };
        await api.Sites.SaveAsync(site);

        // Add blog page
        var page = await BlogPage.CreateAsync(api);
        page.Id = BLOGID;
        page.SiteId = SITEID;
        page.Title = "Blog";
        await api.Pages.SaveAsync(page);

        var post1 = await MyPost.CreateAsync(api);
        post1.Id = POST1ID;
        post1.BlogId = BLOGID;
        post1.SiteId = SITEID;
        post1.Category = "My category";
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
        post2.Id = POST2ID;
        post2.BlogId = BLOGID;
        post2.SiteId = SITEID;
        post2.Category = "My category";
        post2.Title = "My second post";
        post2.Ingress = "My second ingress";
        post2.Body = "My second body";
        await api.Posts.SaveAsync(post2);

        var post3 = await MyPost.CreateAsync(api);
        post3.Id = POST3ID;
        post3.BlogId = BLOGID;
        post3.SiteId = SITEID;
        post3.Category = "My category";
        post3.Title = "My third post";
        post3.Ingress = "My third ingress";
        post3.Body = "My third body";
        await api.Posts.SaveAsync(post3);

        var post4 = await MyCollectionPost.CreateAsync(api);
        post4.BlogId = BLOGID;
        post4.SiteId = SITEID;
        post4.Category = "My category";
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
        post6.Id = POSTDIID;
        post6.BlogId = BLOGID;
        post6.SiteId = SITEID;
        post6.Category = "My category";
        post6.Title = "My Injection Post";
        await api.Posts.SaveAsync(post6);

        var posts = await api.Posts.GetAllDynamicAsync(BLOGID);
        
        // DEBUG: Output saved data details
        Console.WriteLine($"[DEBUG] AddSampleData - SiteId: {SITEID}");
        Console.WriteLine($"[DEBUG] AddSampleData - BlogId: {BLOGID}");
        Console.WriteLine($"[DEBUG] AddSampleData - Posts saved: {posts.Count()}");
        foreach (var post in posts)
        {
            Console.WriteLine($"[DEBUG]   Post: Id={post.Id}, Slug={post.Slug}, BlogId={post.BlogId}");
        }
    }

    public override async Task DisposeAsync()
    {
        
        var posts = await api.Posts.GetAllDynamicAsync(BLOGID);
        foreach (var post in posts)
        {
            await api.Posts.DeleteAsync(post);
        }

        var types = await api.PostTypes.GetAllAsync();
        foreach (var t in types)
        {
            await api.PostTypes.DeleteAsync(t);
        }

        await api.Pages.DeleteAsync(BLOGID);

        var pageTypes = await api.PageTypes.GetAllAsync();
        foreach (var t in pageTypes)
        {
            await api.PageTypes.DeleteAsync(t);
        }

        await api.Sites.DeleteAsync(SITEID);
    }

    [Fact]
    public void IsCached()
    {
        
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(PostTestsMemoryCache) ||
            this.GetType() == typeof(PostTestsDistributedCache));
    }

    [Fact]
    public async Task GetNoneById()
    {
        
        var none = await api.Posts.GetByIdAsync(Snowflake.NewId());

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneBySlug()
    {
        
        var none = await api.Posts.GetBySlugAsync("blog", "none-existing-slug");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneBySlugId()
    {
        
        var none = await api.Posts.GetBySlugAsync(BLOGID, "none-existing-slug");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneBySlugBlog()
    {
        
        var none = await api.Posts.GetBySlugAsync("no-blog", "none-existing-slug");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetNoneBySlugBlogId()
    {
        
        var none = await api.Posts.GetBySlugAsync(Snowflake.NewId(), "none-existing-slug");

        Assert.Null(none);
    }

    [Fact]
    public async Task GetAll()
    {
        
        var posts = await api.Posts.GetAllBySiteIdAsync();

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
    }

    [Fact]
    public async Task GetAllBaseClass()
    {
        var posts = await api.Posts.GetAllBySiteIdAsync<Models.PostBase>(SITEID);

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
    }

    [Fact]
    public async Task GetAllById()
    {
        
        var posts = await api.Posts.GetAllDynamicAsync(BLOGID);

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
    }

    [Fact]
    public async Task GetAllBaseClassById()
    {
        
        var posts = await api.Posts.GetAllAsync<Models.PostBase>(BLOGID);

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
    }

    [Fact]
    public async Task GetAllByIdMissing()
    {
        
        var posts = await api.Posts.GetAllAsync<MissingPost>(BLOGID);

        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    [Fact]
    public async Task GetAllBySlug()
    {
        
        var posts = await api.Posts.GetDynamicAllAsync("blog");

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
    }

    [Fact]
    public async Task GetAllBaseClassBySlug()
    {
        
        var posts = await api.Posts.GetAllBySlugAsync<Models.PostBase>("blog");

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
    }

    [Fact]
    public async Task GetAllBySlugMissing()
    {
        
        var posts = await api.Posts.GetAllBySlugAsync<MissingPost>("blog");

        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    [Fact]
    public async Task GetAllBySlugAndSite()
    {
        
        var posts = await api.Posts.GetDynamicAllAsync("blog", SITEID);

        Assert.NotNull(posts);
        Assert.NotEmpty(posts);
    }

    [Fact]
    public async Task GetAllNoneById()
    {
        
        var posts = await api.Posts.GetAllDynamicAsync(Snowflake.NewId());

        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    [Fact]
    public async Task GetAllNoneBySlug()
    {
        
        var posts = await api.Posts.GetDynamicAllAsync("no-blog");

        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    [Fact]
    public async Task GetAllNoneBySlugAndSite()
    {
        
        var posts = await api.Posts.GetDynamicAllAsync("blog", Snowflake.NewId());

        Assert.NotNull(posts);
        Assert.Empty(posts);
    }

    [Fact]
    public async Task GetGenericById()
    {
        
        var model = await api.Posts.GetByIdAsync<MyPost>(POST1ID);

        Assert.NotNull(model);
        Assert.Equal("my-first-post", model.Slug);
        Assert.Equal("/blog/my-first-post", model.Permalink);
        Assert.Equal("My first body", model.Body.Value);
    }

    [Fact]
    public async Task GetBaseClassById()
    {
        
        var model = await api.Posts.GetByIdAsync<Models.PostBase>(POST1ID);

        Assert.NotNull(model);
        Assert.Equal(typeof(MyPost), model.GetType());
        Assert.Equal("my-first-post", model.Slug);
        Assert.Equal("/blog/my-first-post", model.Permalink);
        Assert.Equal("My first body", ((MyPost)model).Body.Value);
    }

    [Fact]
    public async Task GetBlocksById()
    {
        
        var model = await api.Posts.GetByIdAsync<MyPost>(POST1ID);

        Assert.NotNull(model);
        Assert.Equal(2, model.Blocks.Count);
        Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[0]);
        Assert.IsType<Extend.Blocks.TextBlock>(model.Blocks[1]);
    }

    [Fact]
    public async Task GetMissingById()
    {
        
        var model = await api.Posts.GetByIdAsync<MissingPost>(POST1ID);

        Assert.Null(model);
    }

    [Fact]
    public async Task GetInfoById()
    {
        
        var model = await api.Posts.GetByIdAsync<Models.PostInfo>(POST1ID);

        Assert.NotNull(model);
        Assert.Equal("my-first-post", model.Slug);
        Assert.Equal("/blog/my-first-post", model.Permalink);
        Assert.Empty(model.Blocks);
    }

    [Fact]
    public async Task GetGenericBySlug()
    {
        Console.WriteLine("[DEBUG] GetGenericBySlug: Starting test");
        
        
        // First, verify the blog page exists
        Console.WriteLine("[DEBUG] GetGenericBySlug: Looking up blog page by slug 'blog'");
        var blogPage = await api.Pages.GetBySlugAsync("blog");
        Console.WriteLine($"[DEBUG] GetGenericBySlug: blogPage = {blogPage != null}");
        if (blogPage != null)
        {
            Console.WriteLine($"[DEBUG] GetGenericBySlug: blogPage.Id = {blogPage.Id}");
            Console.WriteLine($"[DEBUG] GetGenericBySlug: BLOGID = {BLOGID}");
        }
        
        // Try to get the post by slug
        Console.WriteLine("[DEBUG] GetGenericBySlug: Calling GetBySlugAsync for post");
        var model = await api.Posts.GetBySlugAsync<MyPost>("blog", "my-first-post");
        Console.WriteLine($"[DEBUG] GetGenericBySlug: model = {model != null}");

        Assert.NotNull(model);
        Assert.Equal("my-first-post", model.Slug);
        Assert.Equal("/blog/my-first-post", model.Permalink);
        Assert.Equal("My first body", model.Body.Value);
    }

    [Fact]
    public async Task GetBaseClassBySlug()
    {
        
        var model = await api.Posts.GetBySlugAsync<Models.PostBase>("blog", "my-first-post");

        Assert.NotNull(model);
        Assert.Equal(typeof(MyPost), model.GetType());
        Assert.Equal("my-first-post", model.Slug);
        Assert.Equal("/blog/my-first-post", model.Permalink);
        Assert.Equal("My first body", ((MyPost)model).Body.Value);
    }

    [Fact]
    public async Task GetMissingBySlug()
    {
        
        var model = await api.Posts.GetBySlugAsync<MissingPost>("blog", "my-first-post");

        Assert.Null(model);
    }

    [Fact]
    public async Task GetInfoBySlug()
    {
        
        var model = await api.Posts.GetBySlugAsync<Models.PostInfo>("blog", "my-first-post");

        Assert.NotNull(model);
        Assert.Equal("my-first-post", model.Slug);
        Assert.Equal("/blog/my-first-post", model.Permalink);
        Assert.Empty(model.Blocks);
    }

    [Fact]
    public async Task GetDynamicById()
    {
        
        var model = await api.Posts.GetByIdAsync(POST1ID);

        Assert.NotNull(model);
        Assert.Equal("my-first-post", model.Slug);
        Assert.Equal("/blog/my-first-post", model.Permalink);
        Assert.Equal("My first body", model.Regions.Body.Value);
    }

    [Fact]
    public async Task GetDynamicBySlug()
    {
        
        var model = await api.Posts.GetBySlugAsync("blog", "my-first-post");

        Assert.NotNull(model);
        Assert.Equal("My first post", model.Title);
        Assert.Equal("My first body", model.Regions.Body.Value);
    }

    [Fact]
    public async Task CheckPermlinkSyntax()
    {
        
        var model = await api.Posts.GetByIdAsync(POST1ID);

        Assert.NotNull(model);
        Assert.NotNull(model.Permalink);
        Assert.StartsWith("/", model.Permalink);
    }

    [Fact]
    public async Task GetCollectionPost()
    {
        
        var post = await api.Posts.GetBySlugAsync<MyCollectionPost>(BLOGID, "my-collection-post");

        Assert.NotNull(post);
        Assert.Equal(3, post.Texts.Count);
        Assert.Equal("Second text", post.Texts[1].Value);
    }

    [Fact]
    public async Task GetBaseClassCollectionPost()
    {
        
        var post = await api.Posts.GetBySlugAsync<Models.PostBase>(BLOGID, "my-collection-post");

        Assert.NotNull(post);
        Assert.Equal(typeof(MyCollectionPost), post.GetType());
        Assert.Equal(3, ((MyCollectionPost)post).Texts.Count);
        Assert.Equal("Second text", ((MyCollectionPost)post).Texts[1].Value);
    }

    [Fact]
    public async Task GetDynamicCollectionPost()
    {
        
        var post = await api.Posts.GetBySlugAsync(BLOGID, "my-collection-post");

        Assert.NotNull(post);
        Assert.Equal(3, post.Regions.Texts.Count);
        Assert.Equal("Second text", post.Regions.Texts[1].Value);
    }

    [Fact]
    public async Task Add()
    {
        
        var count = (await api.Posts.GetAllDynamicAsync(BLOGID)).Count();
        var catCount = (await api.Posts.GetAllCategoriesAsync(BLOGID)).Count();
        var post = await MyPost.CreateAsync(api, "MyPost");
        post.BlogId = BLOGID;
        post.Category = "My category";
        post.Title = "My fourth post";
        post.Ingress = "My fourth ingress";
        post.Body = "My fourth body";

        await api.Posts.SaveAsync(post);

        Assert.Equal(count + 1, (await api.Posts.GetAllDynamicAsync(BLOGID)).Count());
        Assert.Equal(catCount, (await api.Posts.GetAllCategoriesAsync(BLOGID)).Count());
    }

    [Fact]
    public async Task AddWithTags()
    {
        
        var count = (await api.Posts.GetAllDynamicAsync(BLOGID)).Count();
        var catCount = (await api.Posts.GetAllCategoriesAsync(BLOGID)).Count();
        var tagCount = (await api.Posts.GetAllTagsAsync(BLOGID)).Count();

        var post = await MyPost.CreateAsync(api, "MyPost");
        post.BlogId = BLOGID;
        post.SiteId = SITEID;
        post.Category = "My category";
        post.Tags.Add("Testing", "Trying", "Adding");
        post.Title = "My fifth post";
        post.Ingress = "My fifth ingress";
        post.Body = "My fifth body";

        await api.Posts.SaveAsync(post);

        var allDynamic = (await api.Posts.GetAllDynamicAsync(BLOGID)).ToList();
        var allCategories = (await api.Posts.GetAllCategoriesAsync(BLOGID)).ToList();
        var allTags = (await api.Posts.GetAllTagsAsync(BLOGID)).ToList();

        Assert.Equal(count + 1, allDynamic.Count);
        Assert.Equal(catCount, allCategories.Count);
        Assert.Equal(tagCount + 3, allTags.Count);

        post = await api.Posts.GetBySlugAsync<MyPost>(BLOGID, Aero.Cms.Utils.GenerateSlug("My fifth post"));

        Assert.NotNull(post);
        Assert.Equal(3, post.Tags.Count);
        post.Tags.Add("Another tag");

        await api.Posts.SaveAsync(post);

        Assert.Equal(tagCount + 4, (await api.Posts.GetAllTagsAsync(BLOGID)).Count());

        post = await api.Posts.GetBySlugAsync<MyPost>(BLOGID, Aero.Cms.Utils.GenerateSlug("My fifth post"));

        Assert.NotNull(post);
        Assert.Equal(4, post.Tags.Count);
    }

    [Fact]
    public async Task AddDuplicateSlugShouldThrow()
    {
        
        var post = await MyPost.CreateAsync(api);
        post.BlogId = BLOGID;
        post.Title = "My first post";
        post.Published = DateTime.Now;

        await Assert.ThrowsAsync<ValidationException>(async () => { await api.Posts.SaveAsync(post); });
    }

    [Fact]
    public async Task Update()
    {
        
        var post = await api.Posts.GetByIdAsync<MyPost>(POST1ID);

        Assert.NotNull(post);
        Assert.Equal("My first post", post.Title);

        post.Title = "Updated post";
        await api.Posts.SaveAsync(post);

        post = await api.Posts.GetByIdAsync<MyPost>(POST1ID);

        Assert.NotNull(post);
        Assert.Equal("Updated post", post.Title);
    }

    [Fact]
    public async Task UpdateCollectionPost()
    {
        //
        //var p = new MyCollectionPost
        //{
        //    BlogId = BLOGID,
        //    SiteId = SITEID,
        //    Category = "My category",
        //    TypeId = "MyCollectionPost",
        //    Title = "My collection post",
        //    Texts = new List<TextField>
        //    {
        //        new TextField { Value = "First text" },
        //        new TextField { Value = "Second text" },
        //        new TextField { Value = "Third text" }
        //    }
        //};
        //await api.Posts.SaveAsync(p);
        var dynModels = await api.Posts.GetAllAsync<DynamicPost>(BLOGID);
        var models = await api.Posts.GetAllAsync<MyCollectionPost>(BLOGID);
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

    [Fact]
    public async Task Delete()
    {
        
        var post = await api.Posts.GetByIdAsync<MyPost>(POST3ID);
        var count = (await api.Posts.GetAllDynamicAsync(BLOGID)).Count();

        Assert.NotNull(post);

        await api.Posts.DeleteAsync(post);

        Assert.Equal(count - 1, (await api.Posts.GetAllDynamicAsync(BLOGID)).Count());
    }

    [Fact]
    public async Task DeleteById()
    {
        
        var count = (await api.Posts.GetAllDynamicAsync(BLOGID)).Count();

        await api.Posts.DeleteAsync(POST2ID);

        Assert.Equal(count - 1, (await api.Posts.GetAllDynamicAsync(BLOGID)).Count());
    }

    [Fact]
    public async Task GetDIGeneric()
    {
        
        var post = await api.Posts.GetByIdAsync<MyDIPost>(POSTDIID);

        Assert.NotNull(post);
        Assert.Equal("My service value", post.Body.Value);
    }

    [Fact]
    public async Task GetDIDynamic()
    {
        
        var post = await api.Posts.GetByIdAsync(POSTDIID);

        Assert.NotNull(post);
        Assert.Equal("My service value", post.Regions.Body.Value);
    }
}
