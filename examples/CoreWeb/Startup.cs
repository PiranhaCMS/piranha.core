/*
 * Copyright (c) 2017 Håkan Edling
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
using Piranha.ImageSharp;
using Piranha.Local;
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
            services.AddDbContext<Db>(options =>
                options.UseSqlite("Filename=./piranha.coreweb.db"));            
            services.AddSingleton<IStorage, FileStorage>();
            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
            services.AddScoped<IDb, Db>();
            services.AddScoped<IApi, Api>();
            services.AddPiranhaSimpleSecurity(
                new Piranha.AspNetCore.SimpleUser(Piranha.Manager.Permission.All()) {
                    UserName = "admin",
                    Password = "password"
                }
            );
            services.AddPiranhaManager();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider services) {
            loggerFactory.AddConsole();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // Initialize Piranha
            var api = services.GetService<IApi>();
            App.Init(api);

            // Build types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.StandardBlog))
                .AddType(typeof(Models.StandardPage))
                .AddType(typeof(Models.TeaserPage));
            pageTypeBuilder.Build()
                .DeleteOrphans();
            var postTypeBuilder = new Piranha.AttributeBuilder.PostTypeBuilder(api)
                .AddType(typeof(Models.ArticlePost));
            postTypeBuilder.Build()
                .DeleteOrphans();

            // Register middleware
            app.UseStaticFiles();
            app.UsePiranhaSimpleSecurity();
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
                using (var stream = File.OpenRead("assets/seed/startpage.md")) {
                    using (var reader = new StreamReader(stream)) {
                        var startPage = Models.TeaserPage.Create(api);

                        // Add main content
                        startPage.SiteId = site.Id;
                        startPage.Title = "Welcome to Piranha CMS";
                        startPage.MetaKeywords = "Piranha, Piranha CMS, CMS, AspNetCore, DotNetCore, MVC";
                        startPage.MetaDescription = "Piranha is the fun, fast and lightweight framework for developing cms-based web applications with AspNetCore.";
                        startPage.NavigationTitle = "Home";
                        startPage.Heading.Ingress = "The CMS framework with an extra bite";
                        startPage.Body = reader.ReadToEnd();
                        startPage.Published = DateTime.Now;

                        // Add teasers
                        startPage.Teasers.Add(new Models.Regions.Teaser() {
                            Title = "Cross Platform",
                            Image = platformId,
                            Body = "Built for `NetStandard` and `AspNet Core`, Piranha CMS can be run on Windows, Linux and Mac OS X."
                        });
                        startPage.Teasers.Add(new Models.Regions.Teaser() {
                            Title = "Super Fast",
                            Image = stopwatchId,
                            Body = "Designed from the ground up for super-fast performance using `EF Core` and optional Caching."
                        });
                        startPage.Teasers.Add(new Models.Regions.Teaser() {
                            Title = "Open Source",
                            Image = githubId,
                            Body = "Everything is Open Source and released under the `MIT` license for maximum flexibility."
                        });

                        api.Pages.Save(startPage);

                        var docsPage = Models.StandardPage.Create(api);
                        docsPage.SiteId = site.Id;
                        docsPage.SortOrder = 1;
                        docsPage.Title = "Docs";
                        docsPage.RedirectUrl = "https://github.com/PiranhaCMS/piranha.core/wiki";
                        docsPage.Published = DateTime.Now;

                        api.Pages.Save(docsPage);
                    }
                }

                // Add the blog page
                var blogPage = Models.StandardBlog.Create(api);

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
                        var post = Models.ArticlePost.Create(api);

                        // Add main content
                        post.BlogId = blogPage.Id;
                        post.Category = "Uncategorized";
                        post.Title = "My first post";
                        post.MetaKeywords = "First, Blog, AspNetCore, DotNetCore";
                        post.MetaDescription = "The first post ever written by a Piranha";
                        post.Body = reader.ReadToEnd();
                        post.Published = DateTime.Now;
                        post.Tags.Add("Tech", "AspNetCore", "NetCore");  

                        api.Posts.Save(post);                  
                    }
                }
            }
        }
    }
}
