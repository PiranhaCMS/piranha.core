using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Piranha;
using Piranha.AttributeBuilder;
using Piranha.AspNetCore.Identity.SQLite;
using Piranha.Data.EF.SQLite;
using Piranha.Manager.Editor;

namespace RazorWeb
{
    public class Startup
    {
        private readonly IConfiguration _config;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The current configuration</param>
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Service setup
            services.AddPiranha(options =>
            {
                /**
                 * This will enable automatic reload of .cshtml
                 * without restarting the application. However since
                 * this adds a slight overhead it should not be
                 * enabled in production.
                 */
                options.AddRazorRuntimeCompilation = true;

                options.UseCms();
                options.UseManager();

                options.UseFileStorage(naming: Piranha.Local.FileStorageNaming.UniqueFolderNames);
                options.UseImageSharp();
                options.UseTinyMCE();
                options.UseMemoryCache();

                var connectionString = _config.GetConnectionString("piranha");
                options.UseEF<SQLiteDb>(db => db.UseSqlite(connectionString));
                options.UseIdentityWithSeed<IdentitySQLiteDb>(db => db.UseSqlite(connectionString));

                /**
                 * Here you can configure the different permissions
                 * that you want to use for securing content in the
                 * application.
                options.UseSecurity(o =>
                {
                    o.UsePermission("WebUser", "Web User");
                });
                 */

                /**
                 * Here you can specify the login url for the front end
                 * application. This does not affect the login url of
                 * the manager interface.
                options.LoginUrl = "login";
                 */
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApi api)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Initialize Piranha
            App.Init(api);

            // Build content types
            new ContentTypeBuilder(api)
                .AddAssembly(typeof(Startup).Assembly)
                .Build()
                .DeleteOrphans();

            // Configure Tiny MCE
            EditorConfig.FromFile("editorconfig.json");

            // Middleware setup
            app.UsePiranha(options => {
                options.UseManager();
                options.UseTinyMCE();
                options.UseIdentity();
            });

            // Seed test data
            Seed.RunAsync(api).GetAwaiter().GetResult();
        }
    }
}
