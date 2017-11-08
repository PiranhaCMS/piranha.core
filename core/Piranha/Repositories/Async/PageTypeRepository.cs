/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Dapper;
using Newtonsoft.Json;
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace Piranha.Repositories.Async
{
    public class PageTypeRepository : IPageTypeRepository
    {
        #region Members
        private static readonly bool isCreated = typeof(ICreated).IsAssignableFrom(typeof(PageType));
        private static readonly bool isModified = typeof(IModified).IsAssignableFrom(typeof(PageType));
        private readonly IDbConnection db;
        private readonly ICache cache;
        private const string table = "Piranha_PageTypes";
        private const string sort = "Id";
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        public PageTypeRepository(IDbConnection db, ICache cache = null)
        {
            this.db = db;
            this.cache = cache;
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available models</returns>
        public async Task<IEnumerable<Models.PageType>> GetAll(IDbTransaction transaction = null)
        {
            var types = await db.QueryAsync<PageType>($"SELECT * FROM [{table}] ORDER BY [{sort}]", transaction: transaction);
            var models = new List<Models.PageType>();

            foreach (var type in types)
            {
                var model = JsonConvert.DeserializeObject<Models.PageType>(type.Body);

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
        /// <param name="transaction">The optional transaction</param>
        /// <returns></returns>
        public async Task<Models.PageType> GetById(string id, IDbTransaction transaction = null)
        {
            Models.PageType model = cache != null ? cache.Get<Models.PageType>(id) : null;

            if (model == null)
            {
                var type = await db.QueryFirstOrDefaultAsync<PageType>($"SELECT * FROM [{table}] WHERE [Id]=@Id", new
                {
                    Id = id
                }, transaction: transaction);

                if (type != null)
                    model = JsonConvert.DeserializeObject<Models.PageType>(type.Body);

                if (cache != null && model != null)
                    cache.Set(model.Id, model);
            }
            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task Save(Models.PageType model, IDbTransaction transaction = null)
        {
            if (await db.ExecuteScalarAsync<int>($"SELECT COUNT([Id]) FROM [{table}] WHERE [Id]=@Id", model, transaction: transaction) == 0)
                await Add(model, transaction);
            else await Update(model, transaction);

            if (cache != null)
                cache.Remove(model.Id.ToString());
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task Delete(string id, IDbTransaction transaction = null)
        {
            await db.ExecuteAsync($"DELETE FROM [{table}] WHERE [Id]=@Id",
                new { Id = id }, transaction: transaction);

            if (cache != null)
                cache.Remove(id.ToString());
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        public async Task Delete(Models.PageType model, IDbTransaction transaction = null)
        {
            await Delete(model.Id, null);
        }

        #region Private methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        private async Task Add(Models.PageType model, IDbTransaction transaction = null)
        {
            var pageType = new PageType()
            {
                Id = model.Id,
                Body = JsonConvert.SerializeObject(model),
                Created = DateTime.Now,
                LastModified = DateTime.Now
            };

            await db.ExecuteAsync($"INSERT INTO [{table}] ([Id], [Body], [Created], [LastModified]) VALUES (@Id, @Body, @Created, @LastModified)",
                 pageType, transaction: transaction);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        private async Task Update(Models.PageType model, IDbTransaction transaction = null)
        {
            var pageType = await db.QueryFirstAsync<PageType>($"SELECT * FROM [{table}] WHERE [Id]=@Id", new
            {
                Id = model.Id
            });

            pageType.Body = JsonConvert.SerializeObject(model);
            pageType.LastModified = DateTime.Now;

            await db.ExecuteAsync($"UPDATE [{table}] SET [Body]=@Body, [LastModified]=@LastModified WHERE [Id]=@Id",
                pageType, transaction: transaction);
        }
        #endregion
    }
}
