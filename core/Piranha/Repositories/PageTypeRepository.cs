/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Piranha.Models;

namespace Piranha.Repositories
{
    public class PageTypeRepository : IPageTypeRepository
    {
        #region Members
        private readonly IDb db;
        private readonly ICache cache;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        public PageTypeRepository(IDb db, ICache cache = null)
        {
            this.db = db;
            this.cache = cache;
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public IEnumerable<PageType> GetAll()
        {
            var types = db.PageTypes
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .ToList();
            var models = new List<PageType>();

            foreach (var type in types)
            {
                var model = JsonConvert.DeserializeObject<PageType>(type.Body);

                if (cache != null && model != null)
                    cache.Set(model.Id, model);

                models.Add(model);
            }

            return models;
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        public PageType GetById(string id)
        {
            var model = cache?.Get<PageType>(id);

            if (model != null)
            {
                return model;
            }

            var type = db.PageTypes
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id);

            if (type != null)
            {
                model = JsonConvert.DeserializeObject<PageType>(type.Body);
            }

            if (cache != null && model != null)
            {
                cache.Set(model.Id, model);
            }

            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public void Save(PageType model)
        {
            var type = db.PageTypes
                .FirstOrDefault(t => t.Id == model.Id);

            if (type == null)
            {
                type = new Data.PageType
                {
                    Id = model.Id,
                    Created = DateTime.Now
                };
                db.PageTypes.Add(type);
            }
            type.Body = JsonConvert.SerializeObject(model);
            type.LastModified = DateTime.Now;

            db.SaveChanges();

            cache?.Remove(model.Id);
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(string id)
        {
            var type = db.PageTypes
                .FirstOrDefault(t => t.Id == id);

            if (type == null)
            {
                return;
            }

            db.PageTypes.Remove(type);
            db.SaveChanges();

            cache?.Remove(id);
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void Delete(PageType model)
        {
            Delete(model.Id);
        }
    }
}
