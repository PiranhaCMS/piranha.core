using System;
using System.IO;
using Piranha;
using Piranha.Extend.Blocks;

namespace MvcWeb
{
    public static class Seed
    {
        public static void Run(IApi api)
        {
            if (api.Pages.GetStartpage() == null)
            {
                var images = new dynamic []
                {
                    new { id = Guid.NewGuid(), filename = "screenshot2.png" },
                    new { id = Guid.NewGuid(), filename = "logo.png" },
                    new { id = Guid.NewGuid(), filename = "teaser1.png" },
                    new { id = Guid.NewGuid(), filename = "teaser2.png" },
                    new { id = Guid.NewGuid(), filename = "teaser3.png" }
                };

                // Get the default site id
                var siteId = api.Sites.GetDefault().Id;

                // Upload images
                foreach (var image in images)
                {
                    using (var stream = File.OpenRead("seed/" + image.filename))
                    {
                        api.Media.Save(new Piranha.Models.StreamMediaContent() 
                        {
                            Id = image.id,
                            Filename = image.filename,
                            Data = stream
                        });
                    }
                }

                // Create the start page
                var startpage = Models.TeaserPage.Create(api);
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
                    "<p><small>Stable version 5.2.1 - 2018-10-17 - <a href=\"https://github.com/piranhacms/piranha.core/wiki/changelog\" target=\"_blank\">Changelog</a></small></p>";

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
                api.Pages.Save(startpage);

                // Features page
                var featurespage = Models.StandardPage.Create(api);
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
                                    featurespage.Blocks.Add(new HtmlColumnBlock
                                    {
                                        Column1 = App.Markdown.Transform(cols[0].Trim()),
                                        Column2 = App.Markdown.Transform(cols[1].Trim())
                                    });
                                    
                                    if (n < blocks.Length - 1)
                                    {
                                        featurespage.Blocks.Add(new Models.Blocks.SeparatorBlock());
                                    }
                                }
                            }
                        }
                    }
                }
                featurespage.Published = DateTime.Now;
                api.Pages.Save(featurespage);

                // Blog Archive
                var blogpage = Models.BlogArchive.Create(api);
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
                api.Pages.Save(blogpage);

                // Blog Post
                var blogpost = Models.BlogPost.Create(api);
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
                api.Posts.Save(blogpost);
            }
        }
    }
}