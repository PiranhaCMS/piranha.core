using System;
using BasicWeb.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.AspNetCore;
using Piranha.AttributeBuilder;
using Piranha.ImageSharp;
using Piranha.Local;
using Piranha.Manager;
using Piranha.Manager.Binders;

namespace BasicWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config => 
            {
                config.ModelBinderProviders.Insert(0, new AbstractModelBinderProvider());
            });
            services.AddDbContext<Db>(options =>
                options.UseSqlite("Filename=./piranha.db"));            
            services.AddSingleton<IStorage, FileStorage>();
            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
            services.AddScoped<IDb, Db>();
            services.AddScoped<IApi, Api>();
            services.AddPiranhaSimpleSecurity(
                new SimpleUser(Permission.All()) 
                {
                    UserName = "admin",
                    Password = "password"
                }
            );
            services.AddPiranhaManager();

            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Initialize Piranha
            var api = services.GetService<IApi>();
            App.Init(api);

            // Build content types
            var pageTypeBuilder = new PageTypeBuilder(api)
                .AddType(typeof(BlogArchive))
                .AddType(typeof(StandardPage))
                .AddType(typeof(StartPage));
            pageTypeBuilder.Build()
                .DeleteOrphans();
            var postTypeBuilder = new PostTypeBuilder(api)
                .AddType(typeof(BlogPost));
            postTypeBuilder.Build()
                .DeleteOrphans();

            // Register middleware
            app.UseStaticFiles();
            app.UsePiranhaSimpleSecurity();
            app.UsePiranha();
            app.UsePiranhaManager();
            app.UseMvc(routes => 
            {
                routes.MapRoute(name: "areaRoute",
                    template: "{area:exists}/{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    name: "default",
                    template: "{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
