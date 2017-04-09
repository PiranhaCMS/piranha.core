/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Piranha;
using Piranha.Cache;
using Piranha.Local;

namespace CoreWeb
{
    public class Startup
    {
        #region Properties
        /// <summary>
        /// The application config.
        /// </summary>
        public IConfigurationRoot Configuration { get; set; }
        #endregion

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
                o.Connection = new SqliteConnection("Filename=./piranha.db");
                o.Migrate = true;
            });
            //services.AddSingleton<ICache, MemCache>();
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

            // Build types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(Models.MarkdownPage));
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
        /// <param name="db"></param>
        private void Seed(Api api) {
            if (api.Pages.GetAll().Count() == 0) {
                // Get the default site
                var site = api.Sites.GetDefault();

                // Add the startpage
                using (var stream = File.OpenRead("assets/seed/startpage.md")) {
                    using (var reader = new StreamReader(stream)) {
                        var startPage = Models.MarkdownPage.Create(api);
                        startPage.SiteId = site.Id;
                        startPage.Title = "Welcome to Piranha CMS";
                        startPage.Ingress = "The CMS framework with an extra bite";
                        startPage.Body = reader.ReadToEnd();
                        startPage.Published = DateTime.Now;

                        api.Pages.Save(startPage);
                    }
                }
            }
        }
    }
}
