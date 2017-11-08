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
using Piranha.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;

namespace Piranha.Repositories.Async
{
    /// <summary>
    /// Abstract base repository.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public abstract class BaseRepository<T> where T : class, IModel
    {
        #region Members
        protected static readonly bool isCreated = typeof(ICreated).IsAssignableFrom(typeof(T));
        protected static readonly bool isModified = typeof(IModified).IsAssignableFrom(typeof(T));
        protected readonly IDbConnection conn;
        protected readonly ICache cache;
        protected readonly string table;
        protected readonly string sort;
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connection">The current db connection</param>
        /// <param name="tableName">The table name</param>
        /// <param name="sortOrder">The default sort order</param>
        /// <param name="modelCache">The optional model cache</param>
        protected BaseRepository(IDbConnection connection, string tableName, string sortOrder, ICache modelCache = null) {
            conn = connection;
            table = tableName;
            sort = sortOrder;
            cache = modelCache;
        }

        /// <summary>
        /// Gets all available models.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The available models</returns>
        public virtual async Task<IEnumerable<T>> GetAll(IDbTransaction transaction = null) {
            return await conn.QueryAsync<T>($"SELECT * FROM [{table}] ORDER BY [{sort}]", transaction: transaction);
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The model, or NULL if it doesn't exist</returns>
        public virtual async Task<T> GetById(string id, IDbTransaction transaction = null) {
            T model = cache != null ? cache.Get<T>(id.ToString()) : null;

            if (model == null) {
                model = await conn.QueryFirstOrDefaultAsync<T>($"SELECT * FROM [{table}] WHERE [Id]=@Id",
                    new { Id = id }, transaction: transaction);

                if (cache != null && model != null)
                    AddToCache(model);
            }
            return model;
        }

        /// <summary>
        /// Adds or updates the given model in the database
        /// depending on its state.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        public virtual async Task Save(T model, IDbTransaction transaction = null) {
            if (await conn.ExecuteScalarAsync<int>($"SELECT COUNT([Id]) FROM [{table}] WHERE [Id]=@Id", model, transaction: transaction) == 0)
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
        public virtual async Task Delete(string id, IDbTransaction transaction = null) {
           await  conn.ExecuteAsync($"DELETE FROM [{table}] WHERE [Id]=@Id",
                new { Id = id }, transaction: transaction);

            if (cache != null)
                cache.Remove(id.ToString());
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        public virtual async Task Delete(T model, IDbTransaction transaction = null) {
            await Delete(model.Id, null);
        }

        #region Protected methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected abstract Task Add(T model, IDbTransaction transaction = null);

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected abstract Task Update(T model, IDbTransaction transaction = null);

        /// <summary>
        /// Prepares the model for an insert.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected virtual void PrepareInsert(T model, IDbTransaction transaction) {
            // Prepare id
            model.Id = !string.IsNullOrEmpty(model.Id) ? model.Id : Guid.NewGuid().ToString();

            // Prepare created date
            if (isCreated)
                ((ICreated)model).Created = DateTime.Now;

            // Prepare last modified date
            if (isModified)
                ((IModified)model).LastModified = DateTime.Now;
        }

        /// <summary>
        /// Perpares the model for an update.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected virtual void PrepareUpdate(T model, IDbTransaction transaction) {
            // Prepare last modified date
            if (isModified)
                ((IModified)model).LastModified = DateTime.Now;
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected virtual void AddToCache(T model) {
            cache.Set(model.Id.ToString(), model);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected virtual void RemoveFromCache(T model) {
            cache.Remove(model.Id.ToString());
        }
        #endregion
    }
}
