/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Linq;
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
    public class CategoryRepository : RepositoryBase<Data.Category, Models.Category>, ICategoryRepository
    {
		/// <summary>
		/// Default constructor.
		/// </summary>
		/// <param name="db">The current db context</param>
		internal CategoryRepository(Db db) : base(db) { }

		/// <summary>
		/// Gets the category with the specified slug.
		/// </summary>
		/// <param name="slug">The unique slug</param>
		/// <returns>The category</returns>
		public Models.Category GetBySlug(string slug) {
			var result = Query().SingleOrDefault(c => c.Slug == slug);

			if (result != null)
				return Map(result);
			return null;
		}

		/// <summary>
		/// Gets the full category model with the specified id.
		/// </summary>
		/// <param name="id">The unique id</param>
		/// <returns>The category</returns>
		public Models.CategoryModel GetModelById(Guid id) {
			var result = Query().SingleOrDefault(c => c.Id == id);

			if (result != null)
				return MapModel(result);
			return null;
		}

		/// <summary>
		/// Gets the full category model with the specified slug.
		/// </summary>
		/// <param name="slug">The unique slug</param>
		/// <returns>The category</returns>
		public Models.CategoryModel GetModelBySlug(string slug) {
			var result = Query().SingleOrDefault(c => c.Slug == slug);

			if (result != null)
				return MapModel(result);
			return null;
		}

		public void Save(Models.Category model) {
			var category = db.Categories.SingleOrDefault(c => c.Id == model.Id);
			if (category == null) {
				category = new Data.Category() {
					Id = Guid.NewGuid()
				};
				db.Categories.Add(category);
				model.Id = category.Id;
			}
			Module.Mapper.Map<Models.Category, Data.Category>(model, category);

			db.SaveChanges();
		}

		public void Save(Models.CategoryModel model) {
			var category = db.Categories.SingleOrDefault(c => c.Id == model.Id);
			if (category == null) {
				category = new Data.Category() {
					Id = Guid.NewGuid()
				};
				db.Categories.Add(category);
				model.Id = category.Id;
			}
			Module.Mapper.Map<Models.CategoryModel, Data.Category>(model, category);

			db.SaveChanges();
		}

		/// <summary>
		/// Maps the given result to the full category model.
		/// </summary>
		/// <param name="result">The result</param>
		/// <returns>The model</returns>
		protected Models.CategoryModel MapModel(Data.Category result) {
			return Module.Mapper.Map<Data.Category, Models.CategoryModel>(result);
		}
	}
}
