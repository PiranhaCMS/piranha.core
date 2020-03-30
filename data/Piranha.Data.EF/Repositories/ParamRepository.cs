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
using Piranha.Models;

namespace Piranha.Repositories
{
    public class ParamRepository : IParamRepository
    {
        private readonly IDb _db;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        public ParamRepository(IDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public async Task<IEnumerable<Param>> GetAll()
        {
            return await _db.Params
                .AsNoTracking()
                .OrderBy(p => p.Key)
                .Select(p => new Param
                {
                    Id = p.Id,
                    Key = p.Key,
                    Description = p.Description,
                    Value = p.Value,
                    Created = p.Created,
                    LastModified = p.LastModified
                })
                .ToListAsync()
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        public Task<Param> GetById(Guid id)
        {
            return _db.Params
                .AsNoTracking()
                .Select(p => new Param
                {
                    Id = p.Id,
                    Key = p.Key,
                    Description = p.Description,
                    Value = p.Value,
                    Created = p.Created,
                    LastModified = p.LastModified
                })
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Gets the model with the given key.
        /// </summary>
        /// <param name="key">The unique key</param>
        /// <returns>The model</returns>
        public Task<Param> GetByKey(string key) {
            return _db.Params
                .AsNoTracking()
                .Select(p => new Param
                {
                    Id = p.Id,
                    Key = p.Key,
                    Description = p.Description,
                    Value = p.Value,
                    Created = p.Created,
                    LastModified = p.LastModified
                })
                .FirstOrDefaultAsync(p => p.Key == key);
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        public async Task Save(Param model)
        {
            var param = await _db.Params
                .FirstOrDefaultAsync(p => p.Id == model.Id)
                .ConfigureAwait(false);

            if (param == null)
            {
                param = new Data.Param
                {
                    Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid(),
                    Created = DateTime.Now
                };
                await _db.Params.AddAsync(param).ConfigureAwait(false);
            }
            param.Key = model.Key;
            param.Description = model.Description;
            param.Value = model.Value;
            param.LastModified = DateTime.Now;

            await _db.SaveChangesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public async Task Delete(Guid id)
        {
            var param = await _db.Params
                .FirstOrDefaultAsync(p => p.Id == id)
                .ConfigureAwait(false);

            if (param != null)
            {
                _db.Params.Remove(param);
                await _db.SaveChangesAsync().ConfigureAwait(false);
            }
        }
    }
}
