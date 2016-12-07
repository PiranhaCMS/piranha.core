/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Piranha.EF.Data;
using Piranha.Repositories;

namespace Piranha.EF.Repositories
{
    public class PostRepository : RepositoryBase<Data.Post, Models.Post>, IPostRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        internal PostRepository(IDb db) : base(db) { }

        /// <summary>
        /// Gets the post model with the specified id.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The unique id</param>
        /// <returns>The post model</returns>
        public T GetById<T>(Guid id) where T : Models.Post {
            var result = Query().FirstOrDefault(p => p.Id == id);

            if (result != null)
                return Map<T>(result);
            return null;
        }

        /// <summary>
        /// Gets the post models that matches the given
        /// id array.
        /// </summary>
        /// <param name="id">The id array</param>
        /// <returns>The post models</returns>
        public IList<Models.Post> GetById(Guid[] id) {
            var models = new List<Models.Post>();
            var result = Query().Where(p => id.Contains(p.Id)).ToList();

            foreach (var post in result)
                models.Add(Map(post));
            return models;
        }

        /// <summary>
        /// Gets the post models that matches the given
        /// id array.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="id">The id array</param>
        /// <returns>The post models</returns>
        public IList<T> GetById<T>(Guid[] id) where T : Models.Post {
            var models = new List<T>();
            var result = Query().Where(p => id.Contains(p.Id)).ToList();

            foreach (var post in result)
                models.Add(Map<T>(post));
            return models;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <param name="category">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public Models.Post GetBySlug(string category, string slug) {
            var result = Query().FirstOrDefault(p => p.Category.Slug == category && p.Slug == slug);

            if (result != null)
                return Map(result);
            return null;
        }

        /// <summary>
        /// Gets the post model with the specified slug.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="category">The unique category slug</param>
        /// <param name="slug">The unique slug</param>
        /// <returns>The post model</returns>
        public T GetBySlug<T>(string category, string slug) where T : Models.Post {
            var result = Query().FirstOrDefault(p => p.Category.Slug == category && p.Slug == slug);

            if (result != null)
                return Map<T>(result);
            return null;
        }

        /// <summary>
        /// Gets the available post items.
        /// </summary>
        /// <returns>The posts</returns>
        public IList<Models.PostItem> Get() {
            var result = new List<Models.PostItem>();
            var posts = Query()
                .OrderBy(p => p.Published)
                .ThenBy(p => p.Created).ToList();

            foreach (var post in posts) {
                result.Add(MapItem<Models.PostItem>(post));
            }
            return result;            
        }

        /// <summary>
        /// Gets the available post items for the given category id.
        /// </summary>
        /// <param name="id">The unique category id</param>
        /// <returns>The posts</returns>
        public IList<Models.PostItem> GetByCategoryId(Guid id) {
            var result = new List<Models.PostItem>();
            var posts = Query()
                .Where(p => p.CategoryId == id)
                .OrderBy(p => p.Published)
                .ThenBy(p => p.Created).ToList();

            foreach (var post in posts) {
                result.Add(MapItem<Models.PostItem>(post));
            }
            return result;            
        }

        /// <summary>
        /// Gets the available post items for the given category slug.
        /// </summary>
        /// <param name="slug">The unique category slug</param>
        /// <returns>The posts</returns>
        public IList<Models.PostItem> GetByCategorySlug(string slug) {
            var result = new List<Models.PostItem>();
            var posts = Query()
                .Where(p => p.Category.Slug == slug)
                .OrderBy(p => p.Published)
                .ThenBy(p => p.Created).ToList();

            foreach (var post in posts) {
                result.Add(MapItem<Models.PostItem>(post));
            }
            return result;                        
        }
        

        /// <summary>
        /// Gets the base query for the repository.
        /// </summary>
        /// <returns>The query</returns>
        protected override IQueryable<Post> Query() {
            return db.Posts
                .Include(p => p.Category);
        }

        /// <summary>
        /// Maps the given result to the full post model.
        /// </summary>
        /// <param name="result">The result</param>
        /// <returns>The model</returns>
        protected override Models.Post Map(Data.Post result) {
            return Map<Models.Post>(result);
        }

        /// <summary>
        /// Maps the given result to the full post model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="result">The result</param>
        /// <returns>The model</returns>
        private T Map<T>(Data.Post post) where T : Models.Post {
            var model = (T)Activator.CreateInstance<T>();

            // Map basic fields
            Module.Mapper.Map<Data.Post, Models.Post>(post, model);

            // Map category
            model.Category = Module.Mapper.Map<Data.Category, Models.CategoryItem>(post.Category);

            return model;
        }

        /// <summary>
        /// Maps the given result to the full post model.
        /// </summary>
        /// <typeparam name="T">The model type</typeparam>
        /// <param name="result">The result</param>
        /// <returns>The model</returns>
        private T MapItem<T>(Data.Post post) where T : Models.PostItem {
            var model = (T)Activator.CreateInstance<T>();

            // Map basic fields
            Module.Mapper.Map<Data.Post, Models.PostItem>(post, model);

            // Map category
            model.Category = Module.Mapper.Map<Data.Category, Models.CategoryItem>(post.Category);

            return model;
        }
    }
}
