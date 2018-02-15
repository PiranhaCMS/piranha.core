using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Piranha;
using Piranha.ImageSharp;

namespace EmptyWeb
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(config => {
                config.ModelBinderProviders.Insert(0, new Piranha.Manager.Binders.AbstractModelBinderProvider());
            });
            /*
             * Setup: Configure database service.
            services.AddDbContext<Db>(options =>
                ...
            );
            */
            /*
             * Setup: Configure storage service. 
            services.AddSingleton<IStorage, FileStorage>();
             */
            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
            services.AddScoped<IDb, Db>();
            services.AddScoped<IApi, Api>();
            /*
             * Setup: Change to whatever security service you want.
             */
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
