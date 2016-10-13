using System;
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
			services.AddMvc();
			services.AddDbContext<Piranha.EF.Db>(options => options.UseSqlServer(Configuration.GetConnectionString("Piranha")));
			services.AddScoped<IApi, Piranha.EF.Api>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApi api, Piranha.EF.Db db)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }

			// Initialize the piranha application
			App.Init(api, new Piranha.EF.Module());

			// Register middleware
			app.UseStaticFiles();
			app.UsePiranhaPosts();
			app.UsePiranhaArchives();

			app.UseMvc(routes => {
				routes.MapRoute(name: "areaRoute",
					template: "{area:exists}/{controller}/{action}/{id?}",
					defaults: new { controller = "Home", action = "Index" });

				routes.MapRoute(
					name: "default",
					template: "{controller=home}/{action=index}/{id?}");
			});
			Seed(db);
		}

		/// <summary>
		/// Seeds some test data.
		/// </summary>
		/// <param name="db"></param>
		private void Seed(Piranha.EF.Db db) {
			if (db.Categories.Count() == 0) {
				var category = new Piranha.EF.Data.Category() {
					Id = Guid.NewGuid(),
					Title = "Blog",
					ArchiveTitle = "Blog Archive"
				};
				db.Categories.Add(category);

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
