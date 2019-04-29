using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Piranha;
using Piranha.Extend.Blocks;
using Piranha.Services;

namespace MvcWeb
{
    public static class Seed
    {
        public static async Task RunAsync(IApi api)
        {
            if ((await api.Pages.GetStartpageAsync()) == null)
            {
                var folders = new dynamic []
                {
                    new { id = Guid.NewGuid(), name = "Teasers"}
                };

                var images = new dynamic []
                {
                    new { id = Guid.NewGuid(), filename = "screenshot2.png", folderId = Guid.Empty },
                    new { id = Guid.NewGuid(), filename = "logo.png", folderId = Guid.Empty },
                    new { id = Guid.NewGuid(), filename = "teaser1.png", folderId = folders[0].id },
                    new { id = Guid.NewGuid(), filename = "teaser2.png", folderId = folders[0].id },
                    new { id = Guid.NewGuid(), filename = "teaser3.png", folderId = folders[0].id },
                    new { id = Guid.NewGuid(), filename = "8_Hyper-Light-Drifter.png", folderId = Guid.Empty },
                    new { id = Guid.NewGuid(), filename = "Drifter.jpg", folderId = Guid.Empty }
                };

                // Get the default site id
                var siteId = (await api.Sites.GetDefaultAsync()).Id;

                // Create folders
                foreach (var folder in folders)
                {
                    await api.Media.SaveFolderAsync(new Piranha.Models.MediaFolder
                    {
                        Id = folder.id,
                        Name = folder.name
                    });
                }

                // Upload images
                foreach (var image in images)
                {
                    using (var stream = File.OpenRead("seed/" + image.filename))
                    {
                        await api.Media.SaveAsync(new Piranha.Models.StreamMediaContent()
                        {
                            Id = image.id,
                            FolderId = image.folderId != Guid.Empty ? image.folderId : null,
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
                startpage.Blocks.Add(new Models.Blocks.GalleryBlock
                {
                    Title = "Our Image Gallery",
                    Description = "<p>Etiam porta sem malesuada magna mollis euismod. Vestibulum id ligula porta felis euismod semper. Cras justo odio, dapibus ac facilisis in, egestas eget quam.</p>",
                    Items = new List<Piranha.Extend.Block>
                    {
                        new ImageBlock { Body = images[5].id },
                        new ImageBlock { Body = images[6].id }
                    }
                });
                startpage.Blocks.Add(new Models.Blocks.ColumnBlock
                {
                    Items =
                    {
                        new ImageBlock { Body = images[5].id },
                        new HtmlBlock { Body = "<p>Curabitur blandit tempus porttitor. Nullam id dolor id nibh ultricies vehicula ut id elit. Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. Cras mattis consectetur purus sit amet fermentum.</p>" },
                        new HtmlBlock { Body = "<p>Lorem ipsum dolor sit amet, consectetur adipiscing elit. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cras justo odio, dapibus ac facilisis in, egestas eget quam. Praesent commodo cursus magna, vel scelerisque nisl consectetur et.</p>" }
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
                await api.Pages.SaveAsync(blogpage);

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
                await api.Posts.SaveAsync(blogpost);
            }
        }
    }
}