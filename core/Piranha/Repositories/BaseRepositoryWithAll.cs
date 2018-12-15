/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;

namespace Piranha.Repositories
{
    /// <summary>
    /// Abstract base repository.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public abstract class BaseRepositoryWithAll<T> : BaseRepository<T> where T : class, IModel
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        protected BaseRepositoryWithAll(IDb db, ICache cache = null) : base(db, cache) { }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <returns>The available models</returns>
        public virtual IEnumerable<T> GetAll()
        {
            var models = new List<T>();
            var ids = db.Set<T>()
                .AsNoTracking()
                .Select(m => m.Id);

            foreach (var id in ids)
            {
                var model = GetById(id);
                if (model != null)
                    models.Add(model);
            }
            return models;
        }
    }
}
