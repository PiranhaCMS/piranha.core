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

namespace Piranha.Repositories
{
    public class PostTypeRepository : IPostTypeRepository
    {
        private readonly IDb _db;
        private static readonly Dictionary<string, Models.PostType> _types = new Dictionary<string, Models.PostType>();
        private static object _typesMutex = new Object();
        private static bool _isInitialized = false;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        public PostTypeRepository(IDb db) 
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
        public IEnumerable<Models.PostType> GetAll() 
        {
            return _types.Values.OrderBy(t => t.Id);
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns></returns>
        public Models.PostType GetById(string id) 
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
        public void Save(Models.PostType model)
        {
            var type = _db.PostTypes
                .FirstOrDefault(t => t.Id == model.Id);

            if (type == null) 
            {
                type = new Data.PostType 
                {
                    Id = model.Id,
                    Created = DateTime.Now
                };
                _db.PostTypes.Add(type);
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
            var type = _db.PostTypes
                .FirstOrDefault(t => t.Id == id);

            if (type != null) 
            {
                _db.PostTypes.Remove(type);
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
        public void Delete(Models.PostType model) 
        {
            Delete(model.Id);
        }

        /// <summary>
        /// Reloads the page types from the database.
        /// </summary>
        private void Load()
        {
            _types.Clear();

            foreach (var postType in _db.PostTypes)
            {
                _types[postType.Id] = JsonConvert.DeserializeObject<Models.PostType>(postType.Body);
            }
        }
    }
}
