/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Piranha.Repositories
{
    public class SiteTypeRepository : ISiteTypeRepository
    {
        private readonly IDb _db;
        private readonly ICache _cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        public SiteTypeRepository(IDb db, ICache cache = null) 
        {
            _db = db;
            _cache = cache;
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public IEnumerable<Models.SiteType> GetAll() 
        {
            var types = _db.SiteTypes
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .ToList();
            var models = new List<Models.SiteType>();

            foreach (var type in types) 
            {
                var model = JsonConvert.DeserializeObject<Models.SiteType>(type.Body);

                if (_cache != null && model != null)
                    _cache.Set(model.Id, model);

                models.Add(model);
            }
            return models;
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        public Models.SiteType GetById(string id) 
        {
            Models.SiteType model = _cache != null ? _cache.Get<Models.SiteType>(id) : null;

            if (model == null) 
            {
                var type = _db.SiteTypes
                    .AsNoTracking()
                    .FirstOrDefault(t => t.Id == id);

                if (type != null)
                    model = JsonConvert.DeserializeObject<Models.SiteType>(type.Body);

                if (_cache != null && model != null)
                    _cache.Set(model.Id, model);
            }
            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public void Save(Models.SiteType model) 
        {
            var type = _db.SiteTypes
                .FirstOrDefault(t => t.Id == model.Id);

            if (type == null) {
                type = new Data.SiteType 
                {
                    Id = model.Id,
                    Created = DateTime.Now
                };
                _db.SiteTypes.Add(type);
            }
            type.CLRType = model.CLRType;
            type.Body = JsonConvert.SerializeObject(model);
            type.LastModified = DateTime.Now;

            _db.SaveChanges();
            
            if (_cache != null)
                _cache.Remove(model.Id.ToString());
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public void Delete(string id) 
        {
            var type = _db.SiteTypes
                .FirstOrDefault(t => t.Id == id);

            if (type != null) 
            {
                _db.SiteTypes.Remove(type);
                _db.SaveChanges();

                if (_cache != null)
                    _cache.Remove(id.ToString());
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public void Delete(Models.SiteType model) 
        {
            Delete(model.Id);
        }
    }
}
