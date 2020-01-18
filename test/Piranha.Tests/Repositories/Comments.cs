/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Piranha.AttributeBuilder;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Services;

namespace Piranha.Tests.Repositories
{
    [Collection("Integration tests")]
    public class Comments : BaseTests
    {
        private Guid SITE_ID = Guid.NewGuid();
        private Guid PAGE_ID = Guid.NewGuid();
        private Guid BLOG_ID = Guid.NewGuid();
        private Guid NEWS_ID = Guid.NewGuid();
        private Guid BLOGPOST_ID = Guid.NewGuid();
        private Guid NEWSPOST_ID = Guid.NewGuid();

        [PageType(Title = "Blog Archive", IsArchive = true, UseBlocks = false)]
        public class BlogArchive : Page<BlogArchive> {}

        [PostType(Title = "Blog Post")]
        public class BlogPost : Post<BlogPost> {}

        protected override void Init() {
            using (var api = CreateApi()) {
                // Import content types
                new PageTypeBuilder(api)
                    .AddType(typeof(BlogArchive))
                    .Build();
                new PostTypeBuilder(api)
                    .AddType(typeof(BlogPost))
                    .Build();

                // Add site
                var site = new Site() {
                    Id = SITE_ID,
                    Title = "Comment Site",
                    InternalId = "CommentSite",
                    IsDefault = true
                };
                api.Sites.Save(site);

                // Add archive
                var blog = BlogArchive.Create(api);
                blog.Id = BLOG_ID;
                blog.SiteId = SITE_ID;
                blog.Title = "Blog";
                blog.EnableComments = true;
                blog.Published = DateTime.Now;
                api.Pages.Save(blog);

                var news = BlogArchive.Create(api);
                news.Id = NEWS_ID;
                news.SiteId = SITE_ID;
                news.Title = "News";
                blog.EnableComments = true;
                news.Published = DateTime.Now;
                api.Pages.Save(news);

                // Add posts
                var blogPost = BlogPost.Create(api);
                blogPost.Id = BLOGPOST_ID;
                blogPost.BlogId = BLOG_ID;
                blogPost.Category = "The Category";
                blogPost.Title = "Welcome To The Blog";
                blogPost.Published = DateTime.Now;
                api.Posts.Save(blogPost);

                var newsPost = BlogPost.Create(api);
                newsPost.Id = NEWSPOST_ID;
                newsPost.BlogId = NEWS_ID;
                newsPost.Category = "The Category";
                newsPost.Title = "Welcome To The News";
                newsPost.Published = DateTime.Now;
                api.Posts.Save(newsPost);
            }
        }

        protected override void Cleanup() {
            using (var api = CreateApi()) {
                var posts = api.Posts.GetAll(BLOG_ID);
                foreach (var p in posts)
                    api.Posts.Delete(p);

                posts = api.Posts.GetAll(NEWS_ID);
                foreach (var p in posts)
                    api.Posts.Delete(p);

                var pages = api.Pages.GetAll(SITE_ID);
                foreach (var p in pages)
                    api.Pages.Delete(p);

                var sites = api.Sites.GetAll();
                foreach (var s in sites)
                    api.Sites.Delete(s);
            }
        }

        [Fact]
        public async Task AddPostComment()
        {
            using (var api = CreateApi())
            {
                var count = (await api.Posts.GetByIdAsync(BLOGPOST_ID)).CommentCount;

                await api.Posts.SaveCommentAsync(BLOGPOST_ID, new Comment
                {
                    Author = "John Doe",
                    Email = "john@doe.com",
                    Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                });

                var newCount = (await api.Posts.GetByIdAsync(BLOGPOST_ID)).CommentCount;

                Assert.Equal(count + 1, newCount);
            }
        }

        [Fact]
        public async Task AddPostCommentNoAuthor()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () => {
                    await api.Posts.SaveCommentAsync(BLOGPOST_ID, new Comment
                    {
                        Email = "john@doe.com",
                        Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                    });
                });
            }
        }

        [Fact]
        public async Task AddPostCommentNoEmail()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () => {
                    await api.Posts.SaveCommentAsync(BLOGPOST_ID, new Comment
                    {
                        Author = "John Doe",
                        Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                    });
                });

            }
        }

        [Fact]
        public async Task AddPostCommentBadEmail()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () => {
                    await api.Posts.SaveCommentAsync(BLOGPOST_ID, new Comment
                    {
                        Author = "John Doe",
                        Email = "ThisIsNotAnEmail",
                        Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                    });
                });
            }
        }

        [Fact]
        public async Task AddPostCommentAutoApprove()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.CommentsApprove = true;
                }

                var id = Guid.NewGuid();

                await api.Posts.SaveCommentAndVerifyAsync(BLOGPOST_ID, new Comment
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
        }

        [Fact]
        public async Task AddPostCommentAutoUnApprove()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.CommentsApprove = false;
                }

                var id = Guid.NewGuid();

                await api.Posts.SaveCommentAndVerifyAsync(BLOGPOST_ID, new Comment
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
        }

        [Fact]
        public async Task AddPageComment()
        {
            using (var api = CreateApi())
            {
                var count = (await api.Pages.GetByIdAsync(BLOG_ID)).CommentCount;

                await api.Pages.SaveCommentAsync(BLOG_ID, new Comment
                {
                    Author = "John Doe",
                    Email = "john@doe.com",
                    Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                });

                var newCount = (await api.Pages.GetByIdAsync(BLOG_ID)).CommentCount;

                Assert.Equal(count + 1, newCount);
            }
        }

        [Fact]
        public async Task AddPageCommentNoAuthor()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () => {
                    await api.Pages.SaveCommentAsync(BLOG_ID, new Comment
                    {
                        Email = "john@doe.com",
                        Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                    });
                });
            }
        }

        [Fact]
        public async Task AddPageCommentNoEmail()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () => {
                    await api.Pages.SaveCommentAsync(BLOG_ID, new Comment
                    {
                        Author = "John Doe",
                        Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                    });
                });

            }
        }

        [Fact]
        public async Task AddPageCommentBadEmail()
        {
            using (var api = CreateApi())
            {
                await Assert.ThrowsAsync<ValidationException>(async () => {
                    await api.Pages.SaveCommentAsync(BLOG_ID, new Comment
                    {
                        Author = "John Doe",
                        Email = "ThisIsNotAnEmail",
                        Body = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet."
                    });
                });
            }
        }

        [Fact]
        public async Task AddPageCommentAutoApprove()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.CommentsApprove = true;
                }

                var id = Guid.NewGuid();

                await api.Pages.SaveCommentAndVerifyAsync(BLOG_ID, new Comment
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
        }

        [Fact]
        public async Task AddPageCommentAutoUnApprove()
        {
            using (var api = CreateApi())
            {
                using (var config = new Piranha.Config(api))
                {
                    config.CommentsApprove = false;
                }

                var id = Guid.NewGuid();

                await api.Pages.SaveCommentAndVerifyAsync(BLOG_ID, new Comment
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
                storage: storage
            );
        }
    }
}
