using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Askmethat.Aspnet.JsonLocalizer.Extensions;
using Piranha;
using Piranha.Models;
using Piranha.Services;

namespace Piranha.Manager
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddJsonLocalization(options =>
            {
                options.ResourcesPath = "../../../Resources";
            });
            services.AddMvc()
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddPiranha();
            services.AddPiranhaApplication();
            services.AddPiranhaFileStorage();
            services.AddPiranhaImageSharp();
            services.AddPiranhaEF(options =>
                options.UseSqlite("Filename=./piranha.mvcweb.db"));

            services.AddScoped<Services.AliasService>();
            services.AddScoped<Services.ContentTypeService>();
            services.AddScoped<Services.MediaService>();
            services.AddScoped<Services.ModuleService>();
            services.AddScoped<Services.PageService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApi api)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            App.Init(api);

            // Register middleware
            app.UseStaticFiles();
            app.UsePiranha();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });

            // Seed test data
            App.Blocks.Register<MvcWeb.Models.Blocks.SeparatorBlock>();
            App.Blocks.Register<MvcWeb.Models.Blocks.GalleryBlock>();

            // Build content types
            var pageTypeBuilder = new Piranha.AttributeBuilder.PageTypeBuilder(api)
                .AddType(typeof(MvcWeb.Models.BlogArchive))
                .AddType(typeof(MvcWeb.Models.StandardPage))
                .AddType(typeof(MvcWeb.Models.TeaserPage))
                .Build()
                .DeleteOrphans();
            var postTypeBuilder = new Piranha.AttributeBuilder.PostTypeBuilder(api)
                .AddType(typeof(MvcWeb.Models.BlogPost))
                .Build()
                .DeleteOrphans();

            if (api.Aliases.GetAll().Count() == 0)
            {
                var site = api.Sites.GetDefault();

                api.Aliases.Save(new Alias
                {
                    SiteId = site.Id,
                    AliasUrl = "/about-us",
                    RedirectUrl = "/",
                    Type = RedirectType.Temporary
                });
                api.Aliases.Save(new Alias
                {
                    SiteId = site.Id,
                    AliasUrl = "/blog/fast-beautiful-2d-lighting-in-unity",
                    RedirectUrl = "https://medium.com/@tidyui/fast-beautiful-2d-lighting-in-unity-47b76b10447c",
                    Type = RedirectType.Temporary
                });
                api.Aliases.Save(new Alias
                {
                    SiteId = site.Id,
                    AliasUrl = "/customizing-the-html-editor",
                    RedirectUrl = "/blog",
                    Type = RedirectType.Permanent
                });
                api.Aliases.Save(new Alias
                {
                    SiteId = site.Id,
                    AliasUrl = "/docs/api-reference",
                    RedirectUrl = "/docs",
                    Type = RedirectType.Permanent
                });
            }

            MvcWeb.Seed.RunAsync(api).GetAwaiter().GetResult();
        }
    }
}
