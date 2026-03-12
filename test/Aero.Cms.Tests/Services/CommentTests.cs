

using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Xunit;
using Aero.Cms.AttributeBuilder;
using Aero.Cms.Models;

namespace Aero.Cms.Tests.Services;

[Collection("Integration tests")]
public class CommentTestsMemoryCache : CommentTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.MemoryCache((IMemoryCache)services.GetService(typeof(IMemoryCache)));
    }
}

[Collection("Integration tests")]
public class CommentTestsDistributedCache : CommentTests
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        cache = new Cache.DistributedCache((IDistributedCache)services.GetService(typeof(IDistributedCache)));
    }
}

[Collection("Integration tests")]
public class CommentTests : BaseTestsAsync
{
    private string SITEID = Snowflake.NewId();
    private string BLOGID = Snowflake.NewId();
    private string NEWSID = Snowflake.NewId();
    private string BLOGPOSTID = Snowflake.NewId();
    private string NEWSPOSTID = Snowflake.NewId();

    [PageType(Title = "Blog Archive", IsArchive = true, UseBlocks = false)]
    public class BlogArchive : Page<BlogArchive> {}

    [PostType(Title = "Blog Post")]
    public class BlogPost : Post<BlogPost> {}

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        using var api = CreateApi();
        // Import content types
        new ContentTypeBuilder(api)
            .AddType(typeof(BlogArchive))
            .Build();
        new ContentTypeBuilder(api)
            .AddType(typeof(BlogPost))
            .Build();

        // Add site
        var site = new Site
        {
            Id = SITEID,
            Title = "Comment Site",
            InternalId = $"CommentSite{ SITEID }",
            IsDefault = true
        };
        await api.Sites.SaveAsync(site);

        // Add archive
        var blog = await BlogArchive.CreateAsync(api);
        blog.Id = BLOGID;
        blog.SiteId = SITEID;
        blog.Title = "Blog";
        blog.EnableComments = true;
        blog.Published = DateTime.Now;
        await api.Pages.SaveAsync(blog);

        var news = await BlogArchive.CreateAsync(api);
        news.Id = NEWSID;
        news.SiteId = SITEID;
        news.Title = "News";
        blog.EnableComments = true;
        news.Published = DateTime.Now;
        await api.Pages.SaveAsync(news);

        // Add posts
        var blogPost = await BlogPost.CreateAsync(api);
        blogPost.Id = BLOGPOSTID;
        blogPost.BlogId = BLOGID;
        blogPost.Category = "The Category";
        blogPost.Title = "Welcome To The Blog";
        blogPost.Published = DateTime.Now;
        await api.Posts.SaveAsync(blogPost);

        var newsPost = await BlogPost.CreateAsync(api);
        newsPost.Id = NEWSPOSTID;
        newsPost.BlogId = NEWSID;
        newsPost.Category = "The Category";
        newsPost.Title = "Welcome To The News";
        newsPost.Published = DateTime.Now;
        await api.Posts.SaveAsync(newsPost);
    }

    public override async Task DisposeAsync()
    {
        using var api = CreateApi();
        var posts = await api.Posts.GetAllDynamicAsync(BLOGID);
        foreach (var p in posts)
        {
            await api.Posts.DeleteAsync(p);
        }

        posts = await api.Posts.GetAllDynamicAsync(NEWSID);
        foreach (var p in posts)
        {
            await api.Posts.DeleteAsync(p);
        }

        var pages = await api.Pages.GetAllAsync(SITEID);
        foreach (var p in pages)
        {
            await api.Pages.DeleteAsync(p);
        }

        await api.Sites.DeleteAsync(SITEID);
    }

    [Fact]
    public void IsCached()
    {
        using var api = CreateApi();
        Assert.Equal(((Api)api).IsCached,
            this.GetType() == typeof(CommentTestsMemoryCache) ||
            this.GetType() == typeof(CommentTestsDistributedCache));
    }

    [Fact]
    public async Task AddPostComment()
    {
        using var api = CreateApi();
        var count = (await api.Posts.GetByIdAsync(BLOGPOSTID)).CommentCount;

        await api.Posts.SaveCommentAsync(BLOGPOSTID, new PostComment
        {
            Author = "John Doe",
            Email = "john@doe.com",
            Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
        });

        var newCount = (await api.Posts.GetByIdAsync(BLOGPOSTID)).CommentCount;

        Assert.Equal(count + 1, newCount);
    }

    [Fact]
    public async Task CheckPostCommentType()
    {
        using var api = CreateApi();
        var comments = await api.Posts.GetAllCommentsAsync(BLOGPOSTID);

        Assert.True(comments.All(c => c is PostComment));
    }

    [Fact]
    public async Task AddPostCommentNoAuthor()
    {
        using var api = CreateApi();
        await Assert.ThrowsAsync<ValidationException>(async () => {
            await api.Posts.SaveCommentAsync(BLOGPOSTID, new PostComment
            {
                Email = "john@doe.com",
                Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
            });
        });
    }

    [Fact]
    public async Task AddPostCommentNoEmail()
    {
        using var api = CreateApi();
        await Assert.ThrowsAsync<ValidationException>(async () => {
            await api.Posts.SaveCommentAsync(BLOGPOSTID, new PostComment
            {
                Author = "John Doe",
                Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
            });
        });
    }

    [Fact]
    public async Task AddPostCommentBadEmail()
    {
        using var api = CreateApi();
        await Assert.ThrowsAsync<ValidationException>(async () => {
            await api.Posts.SaveCommentAsync(BLOGPOSTID, new PostComment
            {
                Author = "John Doe",
                Email = "ThisIsNotAnEmail",
                Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
            });
        });
    }

    [Fact]
    public async Task AddPostCommentAutoApprove()
    {
        using var api = CreateApi();
        using (var config = new Aero.Cms.Config(api))
        {
            config.CommentsApprove = true;
        }

        var id = Snowflake.NewId();

        await api.Posts.SaveCommentAndVerifyAsync(BLOGPOSTID, new PostComment
        {
            Id = id,
            Author = "John Doe",
            Email = "john@doe.com",
            Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
        });

        var comment = await api.Posts.GetCommentByIdAsync(id);

        Assert.NotNull(comment);
        Assert.True(comment.IsApproved);
    }

    [Fact]
    public async Task AddPostCommentAutoUnApprove()
    {
        using var api = CreateApi();
        using (var config = new Aero.Cms.Config(api))
        {
            config.CommentsApprove = false;
        }

        var id = Snowflake.NewId();

        await api.Posts.SaveCommentAndVerifyAsync(BLOGPOSTID, new PostComment
        {
            Id = id,
            Author = "John Doe",
            Email = "john@doe.com",
            Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
        });

        var comment = await api.Posts.GetCommentByIdAsync(id);

        Assert.NotNull(comment);
        Assert.False(comment.IsApproved);
    }

    [Fact]
    public async Task AddPageComment()
    {
        using var api = CreateApi();
        var count = (await api.Pages.GetByIdAsync(BLOGID)).CommentCount;

        await api.Pages.SaveCommentAsync(BLOGID, new PageComment
        {
            Author = "John Doe",
            Email = "john@doe.com",
            Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
        });

        var newCount = (await api.Pages.GetByIdAsync(BLOGID)).CommentCount;

        Assert.Equal(count + 1, newCount);
    }

    [Fact]
    public async Task CheckPageCommentType()
    {
        using var api = CreateApi();
        var comments = await api.Pages.GetAllCommentsAsync(BLOGID);

        Assert.True(comments.All(c => c is PageComment));
    }

    [Fact]
    public async Task AddPageCommentNoAuthor()
    {
        using var api = CreateApi();
        await Assert.ThrowsAsync<ValidationException>(async () => {
            await api.Pages.SaveCommentAsync(BLOGID, new PageComment
            {
                Email = "john@doe.com",
                Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
            });
        });
    }

    [Fact]
    public async Task AddPageCommentNoEmail()
    {
        using var api = CreateApi();
        await Assert.ThrowsAsync<ValidationException>(async () => {
            await api.Pages.SaveCommentAsync(BLOGID, new PageComment
            {
                Author = "John Doe",
                Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
            });
        });
    }

    [Fact]
    public async Task AddPageCommentBadEmail()
    {
        using var api = CreateApi();
        await Assert.ThrowsAsync<ValidationException>(async () => {
            await api.Pages.SaveCommentAsync(BLOGID, new PageComment
            {
                Author = "John Doe",
                Email = "ThisIsNotAnEmail",
                Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
            });
        });
    }

    [Fact]
    public async Task AddPageCommentAutoApprove()
    {
        using var api = CreateApi();
        using (var config = new Aero.Cms.Config(api))
        {
            config.CommentsApprove = true;
        }

        var id = Snowflake.NewId();

        await api.Pages.SaveCommentAndVerifyAsync(BLOGID, new PageComment
        {
            Id = id,
            Author = "John Doe",
            Email = "john@doe.com",
            Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
        });

        var comment = await api.Pages.GetCommentByIdAsync(id);

        Assert.NotNull(comment);
        Assert.True(comment.IsApproved);
    }

    [Fact]
    public async Task AddPageCommentAutoUnApprove()
    {
        using var api = CreateApi();
        using (var config = new Aero.Cms.Config(api))
        {
            config.CommentsApprove = false;
        }

        var id = Snowflake.NewId();

        await api.Pages.SaveCommentAndVerifyAsync(BLOGID, new PageComment
        {
            Id = id,
            Author = "John Doe",
            Email = "john@doe.com",
            Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
        });

        var comment = await api.Pages.GetCommentByIdAsync(id);

        Assert.NotNull(comment);
        Assert.False(comment.IsApproved);
    }
}
