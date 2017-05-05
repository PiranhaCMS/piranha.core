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
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Piranha;
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
        public IConfigurationRoot Configuration { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="env">The current hosting environment</param>
        public Startup(IHostingEnvironment env) {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {
            services.AddMvc(config => {
                config.ModelBinderProviders.Insert(0, new Piranha.Manager.Binders.AbstractModelBinderProvider());
            });
            services.AddPiranhaDb(o => {
                o.Connection = new SqliteConnection("Filename=./piranha.coreweb.db");
                o.Migrate = true;
            });
            services.AddSingleton<IStorage, FileStorage>();
            services.AddScoped<Api, Api>();
            services.AddPiranhaManager();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, Api api) {
            loggerFactory.AddConsole();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            // Initialize Piranha
            App.Init(api);

            // Config
            using (var config = new Config(api)) {
                config.CacheExpiresPages = 0;
            }

            // Build types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.StandardPage))
                .AddType(typeof(Models.TeaserPage));
            pageTypeBuilder.Build()
                .DeleteOrphans();

            // Register middleware
            app.UseStaticFiles();
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
        private void Seed(Api api) {
            if (api.Pages.GetAll().Count() == 0) {
                // Get the default site
                var site = api.Sites.GetDefault();

                // Add media assets
                var githubId = Guid.NewGuid().ToString();
                var platformId = Guid.NewGuid().ToString();
                var stopwatchId = Guid.NewGuid().ToString();

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
                            Body = "Designed from the ground up for super-fast performance using `Dapper` and optional Caching."
                        });
                        startPage.Teasers.Add(new Models.Regions.Teaser() {
                            Title = "Open Source",
                            Image = githubId,
                            Body = "Everything is Open Source and released under the `MIT` license for maximum flexibility."
                        });

                        api.Pages.Save(startPage);
                    }
                }
            }
        }
    }
}
