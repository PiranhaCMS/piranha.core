/*
 * Copyright (c) .NET Foundation and Contributors
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
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Piranha.Models;

namespace Piranha.Repositories
{
    public class PostTypeRepository : IPostTypeRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        public PostTypeRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public async Task<IEnumerable<PostType>> GetAll()
        {
            var models = new List<PostType>();
            var types = await _db.PostTypes
                .AsNoTracking()
                .OrderBy(t => t.Id)
                .ToListAsync()
                .ConfigureAwait(false);

            foreach (var type in types)
            {
                models.Add(JsonConvert.DeserializeObject<PostType>(type.Body));
            }
            return models;
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique i</param>
        /// <returns></returns>
        public async Task<PostType> GetById(string id)
        {
            var type = await _db.PostTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id)
                .ConfigureAwait(false);

            if (type != null)
            {
                return JsonConvert.DeserializeObject<PostType>(type.Body);
            }
            return null;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task Save(PostType model)
        {
            var type = await _db.PostTypes
                .FirstOrDefaultAsync(t => t.Id == model.Id)
                .ConfigureAwait(false);

            if (type == null) {
                type = new Data.PostType
                {
                    Id = model.Id,
                    Created = DateTime.Now
                };
                await _db.PostTypes.AddAsync(type).ConfigureAwait(false);
            }
            type.CLRType = model.CLRType;
            type.Body = JsonConvert.SerializeObject(model);
            type.LastModified = DateTime.Now;

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(string id)
        {
            var type = await _db.PostTypes
                .FirstOrDefaultAsync(t => t.Id == id)
                .ConfigureAwait(false);

            if (type != null)
            {
                _db.PostTypes.Remove(type);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
