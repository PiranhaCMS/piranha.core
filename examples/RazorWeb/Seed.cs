/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/tidyui/coreweb
 *
 */

using System;
using System.IO;
using System.Threading.Tasks;
using Piranha;
using Piranha.Extend.Blocks;

namespace RazorWeb
{
    public static class Seed
    {
        public static async Task RunAsync(IApi api)
        {
            if ((await api.Pages.GetStartpageAsync()) == null)
            {
                var images = new dynamic []
                {
                    new { id = Guid.NewGuid(), filename = "screenshot2.png" },
                    new { id = Guid.NewGuid(), filename = "logo.png" },
                    new { id = Guid.NewGuid(), filename = "teaser1.png" },
                    new { id = Guid.NewGuid(), filename = "teaser2.png" },
                    new { id = Guid.NewGuid(), filename = "teaser3.png" },
                    new { id = Guid.NewGuid(), filename = "drifter1.png" },
                    new { id = Guid.NewGuid(), filename = "drifter2.jpg" },
                };

                // Get the default site id
                var siteId = (await api.Sites.GetDefaultAsync()).Id;
                var site2Id = Guid.NewGuid();

                await api.Sites.SaveAsync(new Piranha.Models.Site
                {
                    Id = site2Id,
                    Title = "Swedish",
                    Culture = "sv-SE"
                });

                // Upload images
                foreach (var image in images)
                {
                    using (var stream = File.OpenRead("seed/" + image.filename))
                    {
                        await api.Media.SaveAsync(new Piranha.Models.StreamMediaContent()
                        {
                            Id = image.id,
                            Filename = image.filename,
                            Data = stream
                        });
                    }
                }

                // Create the start page
                var startpage = await Models.TeaserPage.CreateAsync(api).ConfigureAwait(false);
                startpage.SiteId = siteId;
                startpage.Title = "Piranha CMS - Open Source, Cross Platform Asp.NET Core CMS";
                startpage.NavigationTitle = "Home";
                startpage.MetaKeywords = "Piranha, Piranha CMS, CMS, AspNetCore, DotNetCore, MVC, .NET, .NET Core";
                startpage.MetaDescription = "Piranha is the fun, fast and lightweight framework for developing cms-based web applications with AspNetCore.";

                // Start page hero
                startpage.Hero.Subtitle = "By developers - for developers";
                startpage.Hero.PrimaryImage = images[1].id;
                startpage.Hero.Ingress =
                    "<p>A lightweight & unobtrusive CMS for ASP.NET Core.</p>" +
                    "<p><small>Stable version 6.1.0 - 2019-05-01 - <a href=\"https://github.com/piranhacms/piranha.core/wiki/changelog\" target=\"_blank\">Changelog</a></small></p>";

                // Teasers
                startpage.Teasers.Add(new Models.Regions.Teaser
                {
                    Title = "Cross Platform",
                    Image = images[2].id,
                    Body = "<p>Built for <code>NetStandard</code> and <code>AspNet Core</code>, Piranha CMS can be run on Windows, Linux and Mac OS X.</p>"
                });
                startpage.Teasers.Add(new Models.Regions.Teaser
                {
                    Title = "Super Fast",
                    Image = images[3].id,
                    Body = "<p>Designed from the ground up for super-fast performance using <code>EF Core</code> and optional Caching.</p>"
                });
                startpage.Teasers.Add(new Models.Regions.Teaser
                {
                    Title = "Open Source",
                    Image = images[4].id,
                    Body = "<p>Everything is Open Source and released under the <code>MIT</code> license for maximum flexibility.</p>"
                });

                // Start page blocks
                startpage.Blocks.Add(new ImageBlock
                {
                    Body = images[0].id
                });
                using (var stream = File.OpenRead("seed/startpage1.md"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        startpage.Blocks.Add(new HtmlBlock
                        {
                            Body = App.Markdown.Transform(reader.ReadToEnd())
                        });
                    }
                }
                startpage.Blocks.Add(new ImageGalleryBlock
                {
                    Items =
                    {
                        new ImageBlock
                        {
                            Body = images[5].id
                        },
                        new ImageBlock
                        {
                            Body = images[6].id
                        }
                    }
                });
                startpage.Blocks.Add(new ColumnBlock
                {
                    Items =
                    {
                        new ImageBlock
                        {
                            Body = images[6].id
                        },
                        new HtmlBlock
                        {
                            Body = "<h3>Ornare Mattis Vulputate</h3><p>Nullam id dolor id nibh ultricies vehicula ut id elit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nullam quis risus eget urna mollis ornare vel eu leo. Lorem ipsum dolor sit amet, consectetur adipiscing elit.</p>"
                        },
                        new ImageBlock
                        {
                            Body = images[5].id
                        }
                    }
                });
                using (var stream = File.OpenRead("seed/startpage2.md"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        startpage.Blocks.Add(new HtmlBlock
                        {
                            Body = App.Markdown.Transform(reader.ReadToEnd())
                        });
                    }
                }
                startpage.Published = DateTime.Now;
                await api.Pages.SaveAsync(startpage);

                startpage.Id = Guid.NewGuid();
                startpage.SiteId = site2Id;
                await api.Pages.SaveAsync(startpage);

                // Features page
                var featurespage = await Models.StandardPage.CreateAsync(api);
                featurespage.SiteId = siteId;
                featurespage.Title = "Features";
                featurespage.Route = "/pagewide";
                featurespage.SortOrder = 1;

                // Features hero
                featurespage.Hero.Subtitle = "Features";
                featurespage.Hero.Ingress = "<p>It's all about who has the sharpest teeth in the pond.</p>";

                // Features blocks
                using (var stream = File.OpenRead("seed/features.md"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var body = reader.ReadToEnd();

                        foreach (var section in body.Split("%"))
                        {
                            var blocks = section.Split("@");

                            for (var n = 0; n < blocks.Length; n++)
                            {
                                var cols = blocks[n].Split("|");

                                if (cols.Length == 1)
                                {
                                    featurespage.Blocks.Add(new HtmlBlock
                                    {
                                        Body = App.Markdown.Transform(cols[0].Trim())
                                    });
                                }
                                else
                                {
                                    featurespage.Blocks.Add(new ColumnBlock
                                    {
                                        Items =
                                        {
                                            new HtmlBlock
                                            {
                                                Body = App.Markdown.Transform(cols[0].Trim())
                                            },
                                            new HtmlBlock
                                            {
                                                Body = App.Markdown.Transform(cols[1].Trim())
                                            }
                                        }
                                    });

                                    if (n < blocks.Length - 1)
                                    {
                                        featurespage.Blocks.Add(new SeparatorBlock());
                                    }
                                }
                            }
                        }
                    }
                }
                featurespage.Published = DateTime.Now;
                await api.Pages.SaveAsync(featurespage);

                // Blog Archive
                var blogpage = await Models.BlogArchive.CreateAsync(api);
                blogpage.Id = Guid.NewGuid();
                blogpage.SiteId = siteId;
                blogpage.Title = "Blog Archive";
                blogpage.NavigationTitle = "Blog";
                blogpage.SortOrder = 2;
                blogpage.MetaKeywords = "Piranha, Piranha CMS, CMS, AspNetCore, DotNetCore, MVC, Blog, News";
                blogpage.MetaDescription = "Read the latest blog posts about Piranha, fast and lightweight framework for developing cms-based web applications with AspNetCore.";

                // Blog Hero
                blogpage.Hero.Subtitle = "Blog Archive";
                blogpage.Hero.Ingress = "<p>Welcome to the blog, the best place to stay up to date with what's happening in the Piranha infested waters.</p>";

                blogpage.Published = DateTime.Now;
                await api.Pages.SaveAsync(blogpage);

                // Blog Post
                var blogpost = await Models.BlogPost.CreateAsync(api);
                blogpost.BlogId = blogpage.Id;
                blogpost.Title = "What is Piranha";
                blogpost.Category = "Piranha CMS";
                blogpost.Tags.Add("welcome");

                using (var stream = File.OpenRead("seed/blogpost.md"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var body = reader.ReadToEnd();

                        foreach (var block in body.Split("@"))
                        {
                            blogpost.Blocks.Add(new HtmlBlock
                            {
                                Body = App.Markdown.Transform(block.Trim())
                            });
                        }
                    }
                }
                blogpost.Published = DateTime.Now;
                await api.Posts.SaveAsync(blogpost);

                // Add some comments
                var comment =  new Piranha.Models.Comment
                {
                    Author = "Håkan Edling",
                    Email = "hakan@tidyui.com",
                    Url = "http://piranhacms.org",
                    Body = "Awesome to see that the project is up and running! Now maybe it's time to start customizing it to your needs. You can find a lot of information in the official docs.",
                    IsApproved = true
                };
                await api.Posts.SaveCommentAsync(blogpost.Id, comment);

                comment.Id = Guid.Empty;
                comment.IsApproved = false;

                await api.Pages.SaveCommentAsync(featurespage.Id, comment);

                // Unpublished Post
                blogpost = await Models.BlogPost.CreateAsync(api);
                blogpost.BlogId = blogpage.Id;
                blogpost.Title = "What is Piranha unpublished";
                blogpost.Category = "Piranha CMS";
                blogpost.Tags.Add("welcome");

                using (var stream = File.OpenRead("seed/blogpost.md"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var body = reader.ReadToEnd();

                        foreach (var block in body.Split("@"))
                        {
                            blogpost.Blocks.Add(new HtmlBlock
                            {
                                Body = App.Markdown.Transform(block.Trim())
                            });
                        }
                    }
                }
                await api.Posts.SaveAsync(blogpost);

                // Scheduled Post
                blogpost = await Models.BlogPost.CreateAsync(api);
                blogpost.BlogId = blogpage.Id;
                blogpost.Title = "What is Piranha scheduled";
                blogpost.Category = "Another category";
                blogpost.Tags.Add("welcome");

                using (var stream = File.OpenRead("seed/blogpost.md"))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var body = reader.ReadToEnd();

                        foreach (var block in body.Split("@"))
                        {
                            blogpost.Blocks.Add(new HtmlBlock
                            {
                                Body = App.Markdown.Transform(block.Trim())
                            });
                        }
                    }
                }
                blogpost.Published = DateTime.Now.AddDays(7);
                await api.Posts.SaveAsync(blogpost);
            }
        }
    }
}