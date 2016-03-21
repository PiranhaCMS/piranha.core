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
using Microsoft.Extensions.DependencyInjection;

namespace Blog
{
	/// <summary>
	/// Starts the web application.
	/// </summary>
	public class Startup
	{
		/// <summary>
		/// Adds all application services.
		/// </summary>
		/// <param name="services">The current services</param>
		public void ConfigureServices(IServiceCollection services) {
			services.AddMvc();
		}

		/// <summary>
		/// Configures the current application services.
		/// </summary>
		/// <param name="app">The current application builder</param>
		/// <param name="env">The hosting environment</param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
			if (env.IsDevelopment()) {
				app.UseDeveloperExceptionPage();
			}

			app.UseIISPlatformHandler();
			app.UseStaticFiles();
			app.UsePiranha(Handle.Pages|Handle.Posts);

			app.UseMvc(routes => {
				routes.MapRoute(name: "areaRoute",
					template: "{area:exists}/{controller}/{action}/{id?}",
					defaults: new { controller = "Home", action = "Index" });

				routes.MapRoute(
					name: "default",
					template: "{controller=home}/{action=index}/{id?}");
			});

			Seed();
		}

		/// <summary>
		/// Seeds the database with some initial data.
		/// </summary>
		private void Seed() {
			using (var db = new Piranha.Db()) {
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
				}

			}
		}

		/// <summary>
		/// Main entry point for the application.
		/// </summary>
		/// <param name="args">Application arg</param>
		public static void Main(string[] args) => WebApplication.Run<Startup>(args);
	}
}
