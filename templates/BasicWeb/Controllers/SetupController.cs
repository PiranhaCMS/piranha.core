using Microsoft.AspNetCore.Mvc;
using Piranha;
using System;

namespace BasicWeb.Controllers
{
    /// <summary>
    /// This controller is only used when the project is first started
    /// and no pages has been added to the database. Feel free to remove it.
    /// </summary>
    public class SetupController : Controller
    {
        private readonly IApi api; 

        public SetupController(IApi api) {
            this.api = api;
        }

        [Route("/")]
        public IActionResult Index() {
            return View();
        }

        [Route("/seed")]
        public IActionResult Seed() {
            // Get the default site
            var site = api.Sites.GetDefault();

            // Add media assets
            var bannerId = Guid.NewGuid();

            using (var stream = System.IO.File.OpenRead("seed/pexels-photo-355423.jpeg")) {
                api.Media.Save(new Piranha.Models.StreamMediaContent() {
                    Id = bannerId,
                    Filename = "pexels-photo-355423.jpeg",
                    Data = stream
                });
            }

            // Add the blog archived
            var blogId = Guid.NewGuid();
            var blogPage = Models.BlogArchive.Create(api);
            blogPage.Id = blogId;
            blogPage.SiteId = site.Id;
            blogPage.Title = "Blog Archive";
            blogPage.MetaKeywords = "Inceptos, Tristique, Pellentesque, Lorem, Vestibulum";
            blogPage.MetaDescription = "Morbi leo risus, porta ac consectetur ac, vestibulum at eros.";
            blogPage.NavigationTitle = "Blog";
            blogPage.Heading.PrimaryImage = bannerId;
            blogPage.Heading.Ingress = "Curabitur blandit tempus porttitor. Maecenas sed diam eget risus varius blandit sit amet non magna.";
            blogPage.Published = DateTime.Now;

            api.Pages.Save(blogPage);

            // Add a blog post
            var postId = Guid.NewGuid();
            var post = Models.BlogPost.Create(api);
            post.Id = postId;
            post.BlogId = blogPage.Id;
            post.Category = "Uncategorized";
            post.Tags.Add("Ornare", "Pellentesque", "Fringilla Ridiculus");  
            post.Title = "Dapibus Cursus Justo";
            post.MetaKeywords = "Nullam, Mollis, Cras, Sem, Ipsum";
            post.MetaDescription = "Aenean lacinia bibendum nulla sed consectetur.";
            post.Body = "<p>Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Maecenas sed diam eget risus varius blandit sit amet non magna. Integer posuere erat a ante venenatis dapibus posuere velit aliquet. Curabitur blandit tempus porttitor. Maecenas faucibus mollis interdum.</p>";
            post.Heading.PrimaryImage = bannerId;
            post.Heading.Ingress = "Sed posuere consectetur est at lobortis. Praesent commodo cursus magna, vel scelerisque nisl consectetur et.";
            post.Published = DateTime.Now;

            api.Posts.Save(post);                  
            
            // Add the startpage
            var startPage = Models.StartPage.Create(api);
            startPage.SiteId = site.Id;
            startPage.Title = "Porta Tortor Euismod";
            startPage.MetaKeywords = "Fusce, Tristique, Nullam, Parturient, Pellentesque";
            startPage.MetaDescription = "Vestibulum id ligula porta felis euismod semper.";
            startPage.NavigationTitle = "Home";
            startPage.Heading.PrimaryImage = bannerId;
            startPage.Heading.Ingress = "Aenean lacinia bibendum nulla sed consectetur.";
            startPage.Body = "<p>Nulla vitae elit libero, a pharetra augue. Curabitur blandit tempus porttitor. Nulla vitae elit libero, a pharetra augue. Donec id elit non mi porta gravida at eget metus. Curabitur blandit tempus porttitor.</p><p>Etiam porta sem malesuada magna mollis euismod. Morbi leo risus, porta ac consectetur ac, vestibulum at eros. Curabitur blandit tempus porttitor. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus.</p>";
            startPage.Published = DateTime.Now;

            // Add teasers
            startPage.Teasers.Add(new Models.Regions.Teaser() {
                Title = "Lorem Consectetur",
                SubTitle = "Ultricies Nullam Cras",
                Body = "Aenean lacinia bibendum nulla sed consectetur. Donec id elit non mi porta gravida at eget metus.",
                PageLink = blogPage
            });
            startPage.Teasers.Add(new Models.Regions.Teaser() {
                Title = "Vestibulum Bibendum",
                SubTitle = "Tortor Cras Tristique",
                Body = "Nullam id dolor id nibh ultricies vehicula ut id elit. Cras justo odio, dapibus ac facilisis in, egestas eget quam.",
                PostLink = post
            });
            api.Pages.Save(startPage);

            return Redirect("~/");
        }
    }
}
