using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
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
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using CoreWebAngular.Models.Fields;
using CoreWebAngular.Models.Blocks;

namespace CoreWebAngular
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config => {
                config.ModelBinderProviders.Insert(0, new Piranha.Manager.Binders.AbstractModelBinderProvider());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IServiceProvider services)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            // Initialize Piranha
            var api = services.GetService<IApi>();
            App.Init(api);

            // Add custom fields
            App.Fields.Register<SizedImageField>();

            // Add custom blocks
            App.Blocks.Register<SliderGroup>();
            App.Blocks.Register<SizedImageBlock>();

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
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();
            app.UseAuthentication();
            app.UsePiranhaManager();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                   template: "{area:exists}/{controller}/{action}/{id?}",
                   defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                // To learn more about options for serving an Angular SPA from ASP.NET Core,
                // see https://go.microsoft.com/fwlink/?linkid=864501

                spa.Options.SourcePath = "ClientApp";

                //spa.UseSpaPrerendering(options =>
                //{
                //    options.BootModulePath = $"{spa.Options.SourcePath}/dist-server/main.bundle.js";
                //    options.BootModuleBuilder = env.IsDevelopment()
                //        ? new AngularCliBuilder(npmScript: "build:ssr")
                //        : null;
                //    options.SupplyData = (context, data) =>
                //    {
                //        // Creates a new value called isHttpsRequest that's passed to TypeScript code
                //        data["isHttpsRequest"] = context.Request.IsHttps;
                //    };
                //    options.ExcludeUrls = new[] { "/sockjs-node" };
                //});

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            Seed(api);
        }

        /// <summary>
        /// Seeds some test data.
        /// </summary>
        /// <param name="api">The current application api</param>
        private void Seed(IApi api)
        {
            if (api.Pages.GetAll().Count() == 0)
            {
                // Get the default site
                var site = api.Sites.GetDefault();

                // Add media assets
                var githubId = Guid.NewGuid();
                var platformId = Guid.NewGuid();
                var stopwatchId = Guid.NewGuid();

                using (var stream = File.OpenRead("assets/seed/github.png"))
                {
                    api.Media.Save(new Piranha.Models.StreamMediaContent()
                    {
                        Id = githubId,
                        Filename = "github.png",
                        Data = stream
                    });
                }
                using (var stream = File.OpenRead("assets/seed/platform.png"))
                {
                    api.Media.Save(new Piranha.Models.StreamMediaContent()
                    {
                        Id = platformId,
                        Filename = "platform.png",
                        Data = stream
                    });
                }
                using (var stream = File.OpenRead("assets/seed/stopwatch.png"))
                {
                    api.Media.Save(new Piranha.Models.StreamMediaContent()
                    {
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
                startPage.Blocks.Add(new HtmlBlock
                {
                    Body =
                        "<h2>What is Piranha.Core</h2>" +
                        "<p>" +
                        "This is a <strong>complete rewrite</strong> of Piranha CMS for <code>NetStandard</code>. The " +
                        "goal of this rewrite is to create a version capable of targeting multiple platforms &amp; " +
                        "frameworks with minimal depenencies, but still provide a flexible & high performance CMS library." +
                        "</p>"
                });
                startPage.Blocks.Add(new HtmlBlock
                {
                    Body =
                        "<p>Piranha is currently built for <code>NetStandard 2.0</code> and uses the following awesome packages:</p>" +
                        "<ul> " +
                        "<li>AutoMapper <code>6.2.1</code></li>" +
                        "<li>Markdig <code>0.14.6</code></li>" +
                        "<li>Microsoft.EntityFrameworkCore <code>2.0.1</code></li>" +
                        "<li>Newtonsoft.Json <code>10.0.3</code></li>" +
                        "</ul>"
                });
                startPage.Blocks.Add(new HtmlBlock
                {
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
                startPage.Teasers.Add(new Models.Regions.Teaser()
                {
                    Title = "Cross Platform",
                    Image = SizedImageField.WithSize(platformId,256),
                    Body = "<p>Built for <code>NetStandard</code> and <code>AspNet Core</code>, Piranha CMS can be run on Windows, Linux and Mac OS X.</p>"
                });
                startPage.Teasers.Add(new Models.Regions.Teaser()
                {
                    Title = "Super Fast",
                    Image = SizedImageField.WithSize(stopwatchId, 256),
                    Body = "<p>Designed from the ground up for super-fast performance using <code>EF Core</code> and optional Caching.</p>"
                });
                startPage.Teasers.Add(new Models.Regions.Teaser()
                {
                    Title = "Open Source",
                    Image = SizedImageField.WithSize(githubId, 256),
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
                using (var stream = File.OpenRead("assets/seed/blogpost.md"))
                {
                    using (var reader = new StreamReader(stream))
                    {
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
                        post.Blocks.Add(new HtmlBlock()
                        {
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
