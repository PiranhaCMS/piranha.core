using Piranha;
using Piranha.Extend;
using Piranha.Extend.Blocks;
using Piranha.Extend.Fields;
using Piranha.Models;
using MvcWeb.Models;

namespace MvcWeb;

/// <summary>
/// Static helper class for seeding test content.
/// </summary>
public static class Seed
{
    /// <summary>
    /// Seeds the test content.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <returns>An awaitable task</returns>
    public static async Task RunAsync(IApi api)
    {
        if ((await api.Pages.GetStartpageAsync()) != null)
        {
            return;
        }

        var images = new Dictionary<string, Guid>();

        // Get the default site
        var site = await api.Sites.GetDefaultAsync();

        // Add media assets
        foreach (var image in Directory.GetFiles("seed"))
        {
            var info = new FileInfo(image);
            var id = Guid.NewGuid();
            images.Add(info.Name, id);

            using (var stream = System.IO.File.OpenRead(image))
            {
                await api.Media.SaveAsync(new Piranha.Models.StreamMediaContent()
                {
                    Id = id,
                    Filename = info.Name,
                    Data = stream
                });
            }
        }

        // Add blog page
        var blogPage = await StandardArchive.CreateAsync(api);
        blogPage.Id = Guid.NewGuid();
        blogPage.SiteId = site.Id;
        blogPage.Title = "Blog Archive";
        blogPage.NavigationTitle = "Blog";
        blogPage.MetaKeywords = "Purus, Amet, Ullamcorper, Fusce";
        blogPage.MetaDescription = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
        blogPage.PrimaryImage = images["woman-in-blue-long-sleeve-dress-standing-beside-brown-wooden-4100766.jpg"];
        blogPage.Excerpt = "Keep yourself updated with the latest and greatest news. All of this knowledge is at your fingertips, what are you waiting for?";
        blogPage.Published = DateTime.Now;

        await api.Pages.SaveAsync(blogPage);

        // Add docs page
        var docsPage = await StandardPage.CreateAsync(api);
        docsPage.Id = Guid.NewGuid();
        docsPage.SiteId = site.Id;
        docsPage.SortOrder = 1;
        docsPage.Title = "Read The Docs";
        docsPage.NavigationTitle = "Docs";
        docsPage.MetaKeywords = "Purus, Amet, Ullamcorper, Fusce";
        docsPage.MetaDescription = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
        docsPage.RedirectUrl = "https://piranhacms.org/docs";
        docsPage.RedirectType = RedirectType.Temporary;
        docsPage.PrimaryImage = images["man-in-red-jacket-standing-on-the-stairs-4390730.jpg"];
        docsPage.Excerpt = "Ready to get started! Then head over to our official documentation and learn more about Piranha and how to use it.";
        docsPage.Published = DateTime.Now;

        await api.Pages.SaveAsync(docsPage);


        // Add start page
        var startPage = await StandardPage.CreateAsync(api);
        startPage.Id = Guid.NewGuid();
        startPage.SiteId = site.Id;
        startPage.Title = "Welcome To Piranha CMS";
        startPage.NavigationTitle = "Home";
        startPage.MetaTitle = "Piranha CMS - Open Source, Cross Platform ASP.NET Core CMS";
        startPage.MetaKeywords = "Purus, Amet, Ullamcorper, Fusce";
        startPage.MetaDescription = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet.";
        startPage.PrimaryImage = images["cute-business-kids-working-on-project-together-surfing-3874121.jpg"];
        startPage.Excerpt = "Welcome to your brand new website. To show some of the features that you have available at your fingertips we have created some example content for you.";
        startPage.Published = DateTime.Now;

        startPage.Blocks.Add(new HtmlBlock
        {
            Body =
                "<h2>Because First Impressions Last</h2>" +
                "<p class=\"lead\">All pages and posts you create have a primary image and excerpt available that you can use both to create nice looking headers for your content, but also when listing or linking to it on your site. These fields are totally optional and can be disabled for each content type.</p>"
        });
        startPage.Blocks.Add(new ColumnBlock
        {
            Items = new List<Block>()
            {
                new ImageBlock
                {
                    Aspect = new SelectField<ImageAspect>
                    {
                        Value = ImageAspect.Widescreen
                    },
                    Body = images["concentrated-little-kids-taking-notes-in-organizer-and-3874109.jpg"]
                },
                new HtmlBlock
                {
                    Body =
                        "<h3>Add, Edit & Rearrange</h3>" +
                        "<p class=\"lead\">Build your content with our powerful and modular block editor that allows you to add, rearrange and layout your content with ease.</p>" +
                        "<p>New content blocks can be installed or created in your project and will be available to use across all content functions. Build complex regions for all of the fixed content you want on your content types.</p>"
                }
            }
        });
        startPage.Blocks.Add(new HtmlBlock
        {
            Body =
                "<h3>Cross-Link Your Content</h3>" +
                "<p>With our new Page and Post Link blocks it's easier than ever to promote, and link to your content across the site. Simple select the content you want to reference and simply use it's basic fields including Primary Image & Excerpt to display it.</p>"
        });
        startPage.Blocks.Add(new ColumnBlock
        {
            Items = new List<Block>
            {
                new PageBlock
                {
                    Body = blogPage
                },
                new PageBlock
                {
                    Body = docsPage
                }
            }
        });
        startPage.Blocks.Add(new HtmlBlock
        {
            Body =
                "<h2>Share Your Images</h2>" +
                "<p>An image says more that a thousand words. With our <strong>Image Gallery</strong> you can easily create a gallery or carousel and share anything you have available in your media library or download new images directly on your page by just dragging them to your browser.</p>"
        });
        startPage.Blocks.Add(new ImageGalleryBlock
        {
            Items = new List<Block>
            {
                new ImageBlock
                {
                    Body = images["cheerful-diverse-colleagues-working-on-laptops-in-workspace-3860809.jpg"]
                },
                new ImageBlock
                {
                    Body = images["smiling-woman-working-in-office-with-coworkers-3860641.jpg"]
                },
                new ImageBlock
                {
                    Body = images["diverse-group-of-colleagues-having-meditation-together-3860619.jpg"]
                }
            }
        });

        await api.Pages.SaveAsync(startPage);

        // Add blog posts
        var post1 = await StandardPost.CreateAsync(api);
        post1.BlogId = blogPage.Id;
        post1.Category = "Magna";
        post1.Tags.Add("Euismod", "Ridiculus");
        post1.Title = "Tortor Magna Ultricies";
        post1.MetaKeywords = "Nibh, Vulputate, Venenatis, Ridiculus";
        post1.MetaDescription = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet. Maecenas faucibus mollis interdum.";
        post1.PrimaryImage = images["smiling-woman-working-in-office-with-coworkers-3860641.jpg"];
        post1.Excerpt = "Maecenas faucibus mollis interdum. Cras justo odio, dapibus ac facilisis in, egestas eget quam. Donec sed odio dui.";
        post1.Published = DateTime.Now;

        post1.Blocks.Add(new HtmlBlock
        {
            Body =
                "<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Vestibulum id ligula porta felis euismod semper. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nullam quis risus eget urna mollis ornare vel eu leo. Nullam id dolor id nibh ultricies vehicula ut id elit. Nullam quis risus eget urna mollis ornare vel eu leo. Aenean eu leo quam. Pellentesque ornare sem lacinia quam venenatis vestibulum.</p>" +
                "<p>Aenean eu leo quam. Pellentesque ornare sem lacinia quam venenatis vestibulum. Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Maecenas sed diam eget risus varius blandit sit amet non magna. Nullam id dolor id nibh ultricies vehicula ut id elit. Maecenas faucibus mollis interdum. Cras mattis consectetur purus sit amet fermentum. Donec ullamcorper nulla non metus auctor fringilla.</p>" +
                "<p>Sed posuere consectetur est at lobortis. Maecenas faucibus mollis interdum. Sed posuere consectetur est at lobortis. Morbi leo risus, porta ac consectetur ac, vestibulum at eros. Nullam id dolor id nibh ultricies vehicula ut id elit. Maecenas faucibus mollis interdum.</p>"
        });
        await api.Posts.SaveAsync(post1);

        var post2 = await StandardPost.CreateAsync(api);
        post2.BlogId = blogPage.Id;
        post2.Category = "Tristique";
        post2.Tags.Add("Euismod", "Ridiculus");
        post2.Title = "Sollicitudin Risus Dapibus";
        post2.MetaKeywords = "Nibh, Vulputate, Venenatis, Ridiculus";
        post2.MetaDescription = "Integer posuere erat a ante venenatis dapibus posuere velit aliquet. Maecenas faucibus mollis interdum.";
        post2.PrimaryImage = images["concentrated-little-kids-taking-notes-in-organizer-and-3874109.jpg"];
        post2.Excerpt = "Donec sed odio dui. Maecenas faucibus mollis interdum. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.";
        post2.Published = DateTime.Now;

        post2.Blocks.Add(new HtmlBlock
        {
            Body =
                "<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Vestibulum id ligula porta felis euismod semper. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Nullam quis risus eget urna mollis ornare vel eu leo. Nullam id dolor id nibh ultricies vehicula ut id elit. Nullam quis risus eget urna mollis ornare vel eu leo. Aenean eu leo quam. Pellentesque ornare sem lacinia quam venenatis vestibulum.</p>" +
                "<p>Aenean eu leo quam. Pellentesque ornare sem lacinia quam venenatis vestibulum. Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Maecenas sed diam eget risus varius blandit sit amet non magna. Nullam id dolor id nibh ultricies vehicula ut id elit. Maecenas faucibus mollis interdum. Cras mattis consectetur purus sit amet fermentum. Donec ullamcorper nulla non metus auctor fringilla.</p>" +
                "<p>Sed posuere consectetur est at lobortis. Maecenas faucibus mollis interdum. Sed posuere consectetur est at lobortis. Morbi leo risus, porta ac consectetur ac, vestibulum at eros. Nullam id dolor id nibh ultricies vehicula ut id elit. Maecenas faucibus mollis interdum.</p>"
        });
        await api.Posts.SaveAsync(post2);

        var post3 = await StandardPost.CreateAsync(api);
        post3.Id = Guid.NewGuid();
        post3.BlogId = blogPage.Id;
        post3.Category = "Piranha";
        post3.Tags.Add("Development", "Release Info");
        post3.Title = "What's New In 10.0";
        post3.Slug = "whats-new";
        post3.MetaKeywords = "Piranha, Version, Information";
        post3.MetaDescription = "Here you can find information about what's included in the current release.";
        post3.PrimaryImage = images["bird-s-eye-view-photography-of-lighted-city-3573383.jpg"];
        post3.Excerpt = "Here you can find information about what's included in the current release.";
        post3.Published = DateTime.Now;

        post3.Blocks.Add(new HtmlBlock
        {
            Body =
                "<p class=\"lead\">Please note that this is a preview version of Piranha CMS.</p>"
        });
        post3.Blocks.Add(new ColumnBlock
        {
            Items = new List<Block>
            {
                new MarkdownBlock
                {
                    Body =
                        "#### Core\n\n" +
                        "* Remove the need to use MARS for SQL Server [#1417](https://github.com/piranhacms/piranha.core/issues/1417)\n" +
                        "* Detect EXIF orientation on mobile pictures [#1442](https://github.com/piranhacms/piranha.core/issues/1442)\n" +
                        "* Update BlobStorage to use Azure.Storage.Blobs [#1564](https://github.com/piranhacms/piranha.core/pull/1564)\n" +
                        "* Update Pomelo.EntityFrameworkCore.MySql [#1646](https://github.com/piranhacms/piranha.core/pull/1646)\n" +
                        "* Add sort order to fields [#1732](https://github.com/piranhacms/piranha.core/issues/1732)\n" +
                        "* Update to .NET 6 [#1733](https://github.com/piranhacms/piranha.core/issues/1733)\n" +
                        "* Use Identify to get image width and height [#1734](https://github.com/piranhacms/piranha.core/pull/1734)\n" +
                        "* Clean up application startup [#1738](https://github.com/piranhacms/piranha.core/issues/1738)\n" +
                        "* Add markdown block [#1744](https://github.com/piranhacms/piranha.core/issues/1744)\n\n" +
                        "#### Manager\n\n" +
                        "* Add content settings (with region support) [#1524](https://github.com/piranhacms/piranha.core/issues/1524)\n" +
                        "* Update Summernote package [#1730](https://github.com/piranhacms/piranha.core/issues/1730)\n" +
                        "* Manager security update [#1741](https://github.com/piranhacms/piranha.core/issues/1741)\n\n" +
                        "#### Bugfixes\n\n" +
                        "* Cannot access disposed object [#1701](https://github.com/piranhacms/piranha.core/issues/1701)\n" +
                        "* Invalid PageField URL in Manager [#1705](https://github.com/piranhacms/piranha.core/issues/1705)\n"
                },
                new ImageBlock
                {
                    Body = images["person-looking-at-phone-and-at-macbook-pro-1181244.jpg"]
                }
            }
        });

        await api.Posts.SaveAsync(post3);

        var comment = new Piranha.Models.PostComment
        {
            Author = "Håkan Edling",
            Email = "hakan@tidyui.com",
            Url = "http://piranhacms.org",
            Body = "Awesome to see that the project is up and running! Now maybe it's time to start customizing it to your needs. You can find a lot of information in the official docs.",
            IsApproved = true
        };
        await api.Posts.SaveCommentAsync(post3.Id, comment);
    }
}
