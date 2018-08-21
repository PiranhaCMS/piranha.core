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
        private static readonly Dictionary<string, Models.SiteType> _types = new Dictionary<string, Models.SiteType>();
        private static object _typesMutex = new Object();
        private static bool _isInitialized = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        public SiteTypeRepository(IDb db) 
        {
            _db = db;

            if (!_isInitialized)
            {
                lock (_typesMutex)
                {
                    if (!_isInitialized)
                    {
                        Load();

                        _isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public IEnumerable<Models.SiteType> GetAll() 
        {
            return _types.Values.OrderBy(t => t.Id);
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        public Models.SiteType GetById(string id) 
        {
            if (_types.TryGetValue(id, out var type))
                return type;
            return null;
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
            
            lock (_typesMutex)
            {
                Load();
            }
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

                lock (_typesMutex)
                {
                    Load();
                }
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

        /// <summary>
        /// Reloads the page types from the database.
        /// </summary>
        private void Load()
        {
            _types.Clear();

            foreach (var siteType in _db.SiteTypes)
            {
                _types[siteType.Id] = JsonConvert.DeserializeObject<Models.SiteType>(siteType.Body);
            }
        }
    }
}
