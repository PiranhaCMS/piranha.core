/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha;
using System;
using System.Linq;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;

namespace Blog
{
	/// <summary>
	/// Starts the web application.
	/// </summary>
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
		/// <param name="appEnv">The current application environment</param>
		public Startup(IHostingEnvironment env, IApplicationEnvironment appEnv) {
			var builder = new ConfigurationBuilder()
				.SetBasePath(appEnv.ApplicationBasePath)
				.AddJsonFile("appsettings.json");
			Configuration = builder.Build();
		}

		/// <summary>
		/// Adds all application services.
		/// </summary>
		/// <param name="services">The current services</param>
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc();
			services.AddEntityFramework()
				.AddSqlServer()
				.AddDbContext<Db>(options => 
					options.UseSqlServer(Configuration["Data:Piranha:ConnectionString"]));
			services.AddScoped<Api>();
		}

		/// <summary>
		/// Configures the current application services.
		/// </summary>
		/// <param name="app">The current application builder</param>
		/// <param name="env">The hosting environment</param>
		/// <param name="db">The configured db context</param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env, Db db) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseIISPlatformHandler();
			app.UseStaticFiles();
			app.UsePiranha();

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
		/// Seeds the database with some initial data.
		/// </summary>
		/// <param name="db">The current db context</param>
		private void Seed(Db db) {
			if (db.PageTypes.Count() == 0) {
				//
				// Create page type
				//
				var type = new Piranha.Data.PageType() {
					Id = Guid.NewGuid(),
					Name = "Standard page",
					InternalId = "Standard"
				};
				type.Fields.Add(new Piranha.Data.PageTypeField() {
					Id = Guid.NewGuid(),
					TypeId = type.Id,
					FieldType = Piranha.Data.FieldType.Region,
					Name = "Main body",
					InternalId = "Body",
					SortOrder = 0,
					CLRType = typeof(Piranha.Extend.Regions.HtmlRegion).FullName
				});
				db.PageTypes.Add(type);

				//
				// Create page
				//
				var page = new Piranha.Data.Page() {
					Id = Guid.NewGuid(),
					TypeId = type.Id,
					Title = "Start",
					Published = DateTime.Now
				};
				page.Fields.Add(new Piranha.Data.PageField() {
					Id = Guid.NewGuid(),
					TypeId = type.Fields[0].Id,
					ParentId = page.Id,
					Value = new Piranha.Extend.Regions.HtmlRegion() {
						Value = "<p>Lorem ipsum</p>"
					}
				});
				db.Pages.Add(page);

				//
				// Create category
				//
				var category = new Piranha.Data.Category() {
					Id = Guid.NewGuid(),
					Title = "Blog",
					HasArchive = true,
					ArchiveTitle = "Blog Archive"
				};
				db.Categories.Add(category);

				//
				// Create post type
				//
				var postType = new Piranha.Data.PostType() {
					Id = Guid.NewGuid(),
					Name = "Standard post",
					InternalId = "Standard"
				};
				postType.Fields.Add(new Piranha.Data.PostTypeField() {
					Id = Guid.NewGuid(),
					TypeId = postType.Id,
					FieldType = Piranha.Data.FieldType.Region,
					Name = "Main body",
					InternalId = "Body",
					SortOrder = 0,
					CLRType = typeof(Piranha.Extend.Regions.HtmlRegion).FullName
				});
				db.PostTypes.Add(postType);

				//
				// Create post
				//
				var post = new Piranha.Data.Post() {
					Id = Guid.NewGuid(),
					TypeId = postType.Id,
					CategoryId = category.Id,
					Title = "My first post",
					Excerpt = "Etiam porta sem malesuada magna mollis euismod.",
					Published = DateTime.Now
				};
				post.Fields.Add(new Piranha.Data.PostField() {
					Id = Guid.NewGuid(),
					TypeId = postType.Fields[0].Id,
					ParentId = post.Id,
					Value = new Piranha.Extend.Regions.HtmlRegion() {
						Value = "<p>Curabitur blandit tempus porttitor.</p>"
					}
				});
				db.Posts.Add(post);

				db.SaveChanges();

				//
				// Create page
				//
				var page2 = new Piranha.Data.Page() {
					Id = Guid.NewGuid(),
					TypeId = type.Id,
					Title = "About",
					SortOrder = 1,
					Published = DateTime.Now
				};
				page2.Fields.Add(new Piranha.Data.PageField() {
					Id = Guid.NewGuid(),
					TypeId = type.Fields[0].Id,
					ParentId = page2.Id,
					Value = new Piranha.Extend.Regions.HtmlRegion() {
						Value = "<p>Morbi leo risus, porta ac consectetur ac, vestibulum at eros.</p>"
					}
				});
				db.Pages.Add(page2);
				db.SaveChanges();
			}
		}

		/// <summary>
		/// Main entry point for the application.
		/// </summary>
		/// <param name="args">Application arg</param>
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
