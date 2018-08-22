/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Piranha;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.AspNetCore.Identity.SQLServer;
using Piranha.AspNetCore.Identity.MySQL;
using Piranha.Extend.Blocks;
using Piranha.ImageSharp;
using Piranha.Local;
using Piranha.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CoreWeb
{
    public class Startup
    {
        /// <summary>
        /// The application config.
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="env">The current hosting environment</param>
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services) {
            services.AddMvc(config => {
                config.ModelBinderProviders.Insert(0, new Piranha.Manager.Binders.AbstractModelBinderProvider());
            });
            services.AddPiranhaFileStorage();
            services.AddPiranhaImageSharp();
            
            // SQLite
            services.AddPiranhaEF(options => options.UseSqlite("Filename=./piranha.coreweb.db"));
            services.AddPiranhaIdentityWithSeed<IdentitySQLiteDb>(options => options.UseSqlite("Filename=./piranha.coreweb.db"));

            // SQLServer
            //services.AddPiranhaEF(options => options.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=piranha.core;Integrated Security=True;MultipleActiveResultSets=True"));
            //services.AddPiranhaIdentityWithSeed<IdentitySQLServerDb>(options => options.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=piranha.core;Integrated Security=True;MultipleActiveResultSets=True"));

            // MySQL
            //services.AddPiranhaEF(options => options.UseMySql("server=localhost;port=3306;database=piranha;uid=root;password=password"));
            //services.AddPiranhaIdentityWithSeed<IdentityMySQLDb>(options => options.UseMySql("server=localhost;port=3306;database=piranha;uid=root;password=password"));
            services.AddPiranhaManager();
            services.AddPiranhaApplication();

            // Initialize Piranha
            App.Init();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApi api) {
            loggerFactory.AddConsole();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // Add custom blocks
            App.Blocks.Register<Models.Blocks.SliderGroup>();
            App.Blocks.Register<Models.Blocks.SliderItem>();

            // Build types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.BlogArchive))
                .AddType(typeof(Models.StandardPage))
                .AddType(typeof(Models.TeaserPage));
            pageTypeBuilder.Build()
                .DeleteOrphans();
            var postTypeBuilder = new Piranha.AttributeBuilder.PostTypeBuilder(api)
                .AddType(typeof(Models.BlogPost));
            postTypeBuilder.Build()
                .DeleteOrphans();
            var siteTypeBuilder = new Piranha.AttributeBuilder.SiteTypeBuilder(api)
                .AddType(typeof(Models.StandardSite));
            siteTypeBuilder.Build()
                .DeleteOrphans();

            // Register middleware
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UsePiranha();
            app.UsePiranhaManager();
            app.UseMvc(routes => {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });

            Seed(api);
        }

        /// <summary>
        /// Seeds some test data.
        /// </summary>
        /// <param name="api">The current application api</param>
        private void Seed(IApi api) {
            if (api.Pages.GetAll().Count() == 0) {
                // Get the default site
                var site = api.Sites.GetDefault();

                // Add media assets
                var githubId = Guid.NewGuid();
                var platformId = Guid.NewGuid();
                var stopwatchId = Guid.NewGuid();

                using (var stream = File.OpenRead("assets/seed/github.png")) {
                    api.Media.Save(new Piranha.Models.StreamMediaContent() {
                        Id = githubId,
                        Filename = "github.png",
                        Data = stream
                    });
                }
                using (var stream = File.OpenRead("assets/seed/platform.png")) {
                    api.Media.Save(new Piranha.Models.StreamMediaContent() {
                        Id = platformId,
                        Filename = "platform.png",
                        Data = stream
                    });
                }
                using (var stream = File.OpenRead("assets/seed/stopwatch.png")) {
                    api.Media.Save(new Piranha.Models.StreamMediaContent() {
                        Id = stopwatchId,
                        Filename = "stopwatch.png",
                        Data = stream
                    });
                }

                // Add the startpage
                var startPage = Models.TeaserPage.Create(api);

                // Add meta info
                startPage.SiteId = site.Id;
                startPage.Title = "Welcome to Piranha CMS";
                startPage.MetaKeywords = "Piranha, Piranha CMS, CMS, AspNetCore, DotNetCore, MVC";
                startPage.MetaDescription = "Piranha is the fun, fast and lightweight framework for developing cms-based web applications with AspNetCore.";
                startPage.NavigationTitle = "Home";
                startPage.Heading.Ingress = "The CMS framework with an extra bite";
                startPage.Blocks.Add(new HtmlBlock {
                    Body =
                        "<h2>What is Piranha.Core</h2>" +
                        "<p>" +
                        "This is a <strong>complete rewrite</strong> of Piranha CMS for <code>NetStandard</code>. The " +
                        "goal of this rewrite is to create a version capable of targeting multiple platforms &amp; " +
                        "frameworks with minimal depenencies, but still provide a flexible & high performance CMS library." +
                        "</p>"
                });
                startPage.Blocks.Add(new HtmlBlock {
                    Body =
                        "<p>Piranha is currently built for <code>NetStandard 2.0</code> and uses the following awesome packages:</p>" +
                        "<ul> " +
                        "<li>AutoMapper <code>7.0.1</code></li>" +
                        "<li>Markdig <code>0.15.0</code></li>" +
                        "<li>Microsoft.AspNetCore <code>2.1.1</code></li>" +
                        "<li>Microsoft.EntityFrameworkCore <code>2.1.1</code></li>" +
                        "<li>Newtonsoft.Json <code>11.0.2</code></li>" +
                        "</ul>"
                });
                startPage.Blocks.Add(new HtmlBlock {
                    Body =
                        "<h2>Licensing</h2>" +
                        "<p>" +
                            "Piranha CMS is released under the <strong>MIT</strong> license. It is a permissive free " +
                            "software license, meaning that it permits reuse within proprietary software provided all " +
                            "copies of the licensed software include a copy of the MIT License terms and the copyright " +
                            "notice." +
                        "</p>"
                });
                startPage.Published = DateTime.Now;

                // Add teasers
                startPage.Teasers.Add(new Models.Regions.Teaser() {
                    Title = "Cross Platform",
                    Image = platformId,
                    Body = "<p>Built for <code>NetStandard</code> and <code>AspNet Core</code>, Piranha CMS can be run on Windows, Linux and Mac OS X.</p>"
                });
                startPage.Teasers.Add(new Models.Regions.Teaser() {
                    Title = "Super Fast",
                    Image = stopwatchId,
                    Body = "<p>Designed from the ground up for super-fast performance using <code>EF Core</code> and optional Caching.</p>"
                });
                startPage.Teasers.Add(new Models.Regions.Teaser() {
                    Title = "Open Source",
                    Image = githubId,
                    Body = "<p>Everything is Open Source and released under the <code>MIT</code> license for maximum flexibility.</p>"
                });
                
                api.Pages.Save(startPage);

                var docsPage = Models.StandardPage.Create(api);
                docsPage.SiteId = site.Id;
                docsPage.SortOrder = 1;
                docsPage.Title = "Docs";
                docsPage.RedirectUrl = "https://github.com/PiranhaCMS/piranha.core/wiki";
                docsPage.Published = DateTime.Now;

                api.Pages.Save(docsPage);

                // Add the blog page
                var blogPage = Models.BlogArchive.Create(api);

                blogPage.SiteId = site.Id;
                blogPage.Title = "Blog Archive";
                blogPage.SortOrder = 1;
                blogPage.MetaKeywords = "Piranha, Piranha CMS, CMS, AspNetCore, DotNetCore, MVC, Blog";
                blogPage.MetaDescription = "Read the latest blog posts about Piranha, fast and lightweight framework for developing cms-based web applications with AspNetCore.";
                blogPage.NavigationTitle = "Blog";
                blogPage.Body = "Welcome to the blog, the best place to stay up to date with what's happening in the Piranha infested waters.";
                blogPage.Published = DateTime.Now;

                api.Pages.Save(blogPage);

                // Add a blog post
                using (var stream = File.OpenRead("assets/seed/blogpost.md")) {
                    using (var reader = new StreamReader(stream)) {
                        var post = Models.BlogPost.Create(api);

                        // Add main content
                        post.BlogId = blogPage.Id;
                        post.Category = "Uncategorized";
                        post.Title = "My first post";
                        post.MetaKeywords = "First, Blog, AspNetCore, DotNetCore";
                        post.MetaDescription = "The first post ever written by a Piranha";
                        post.Heading.Ingress = 
                            "<p>" +
                            "Fusce dapibus, tellus ac cursus commodo, tortor mauris condimentum nibh, ut fermentum massa justo sit amet risus. " +
                            "Etiam porta sem malesuada magna mollis euismod. Lorem ipsum dolor sit amet, consectetur adipiscing elit." +
                            "</p>";
                        post.Blocks.Add(new HtmlBlock() {
                            Body = App.Markdown.Transform(reader.ReadToEnd())
                        });
                        post.Published = DateTime.Now;
                        post.Tags.Add("Tech", "AspNetCore", "NetCore");  

                        api.Posts.Save(post);                  
                    }
                }
            }
        }
    }
}
