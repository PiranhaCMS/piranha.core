using System;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Piranha;

namespace Blog
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
            services.AddMvc();
            services.AddPiranhaEF(options => options.UseSqlite("Filename=./blog.db"));
            services.AddPiranhaManager();
            services.AddSingleton<IStorage, Piranha.Local.Storage.FileStorage>();
            services.AddScoped<IApi, Api>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApi api, Piranha.EF.Db db) {
            loggerFactory.AddConsole();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

            // Initialize Piranha
            App.Init(api);

            // Build types
            var pageTypeBuilder = new Piranha.Builder.Json.PageTypeBuilder(api)
                .AddJsonFile("piranha.json");
            pageTypeBuilder.Build();
            var blockTypeBuilder = new Piranha.Builder.Json.BlockTypeBuilder(api)
                .AddJsonFile("piranha.json");
            blockTypeBuilder.Build();
            var attrTypeBuilder = new Piranha.Builder.Attribute.PageTypeBuilder(api, loggerFactory)
                .AddType(typeof(Models.ContentPageModel));
            attrTypeBuilder.Build();

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
            Seed(api, db);
        }



        /// <summary>
        /// Seeds some test data.
        /// </summary>
        /// <param name="db"></param>
        private void Seed(IApi api, Piranha.EF.Db db) {
            if (api.Media.Get().Count == 0) {
            }

            if (api.Categories.Get().Count == 0) {
                // Add the main image
                var content = new Piranha.Models.StreamMediaContent() {
                    Id = Guid.NewGuid(),
                    Filename = "banner.jpg",
                    ContentType = "image/jpg"
                };
                using (var stream = File.OpenRead("assets/seed/banner.jpg")) {
                    content.Data = stream;
                    api.Media.Save(content);
                }

                // Add the startpage
                using (var stream = File.OpenRead("assets/seed/startpage.md")) {
                    using (var reader = new StreamReader(stream)) {
                        var startPage = Models.ContentPageModel.Create("Content");
                        startPage.Title = "Welcome to Piranha CMS";
                        startPage.Settings.Ingress = "The CMS framework with an extra bite";
                        startPage.Settings.PrimaryImage = content.Id;
                        startPage.Body = reader.ReadToEnd();
                        startPage.Published = DateTime.Now;
                        api.Pages.Save(startPage);
                    }
                }

                // Add the blog category
                var category = new Piranha.Models.Category() {
                    Id = Guid.NewGuid(),
                    Title = "Blog",
                    ArchiveTitle = "Blog Archive"
                };
                api.Categories.Save(category);

                // Add a post
                var post = new Piranha.EF.Data.Post() {
                    CategoryId = category.Id,
                    Title = "My first post",
                    Excerpt = "Etiam porta sem malesuada magna mollis euismod.",
                    Body = "<p>Praesent commodo cursus magna, vel scelerisque nisl consectetur et. Morbi leo risus, porta ac consectetur ac, vestibulum at eros. Integer posuere erat a ante venenatis dapibus posuere velit aliquet. Nullam id dolor id nibh ultricies vehicula ut id elit.</p>",
                    Published = DateTime.Now
                };
                db.Posts.Add(post);
                db.SaveChanges();
            }
        }
    }
}
