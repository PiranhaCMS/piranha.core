/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Piranha.Data;

namespace Piranha.Repositories
{
    /// <summary>
    /// Abstract base repository.
    /// </summary>
    /// <typeparam name="T">The model type</typeparam>
    public abstract class BaseRepository<T> where T : class, IModel
    {
        protected static readonly bool isCreated = typeof(ICreated).IsAssignableFrom(typeof(T));
        protected static readonly bool isModified = typeof(IModified).IsAssignableFrom(typeof(T));
        protected readonly IDb db;
        protected readonly ICache cache;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        protected BaseRepository(IDb db, ICache cache = null)
        {
            this.db = db;
            this.cache = cache;
        }

        /// <summary>
        /// Gets the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The model, or null if it doesn't exist</returns>
        public virtual T GetById(Guid id)
        {
            T model = cache != null ? cache.Get<T>(id.ToString()) : null;

            if (model == null)
            {
                model = db.Set<T>()
                    .AsNoTracking()
                    .FirstOrDefault(m => m.Id == id);

                if (model != null)
                    App.Hooks.OnLoad<T>(model);

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
        public virtual void Save(T model)
        {
            App.Hooks.OnBeforeSave<T>(model);

            if (db.Set<T>().Count(m => m.Id == model.Id) == 0)
                Add(model);
            else Update(model);

            db.SaveChanges();

            App.Hooks.OnAfterSave<T>(model);

            if (cache != null)
                cache.Remove(model.Id.ToString());
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public virtual void Delete(Guid id)
        {
            var model = db.Set<T>().FirstOrDefault(m => m.Id == id);
            if (model != null)
            {
                App.Hooks.OnBeforeDelete<T>(model);

                db.Set<T>().Remove(model);
                db.SaveChanges();

                App.Hooks.OnAfterDelete<T>(model);
            }

            if (cache != null)
                cache.Remove(id.ToString());
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="model">The model</param>
        public virtual void Delete(T model)
        {
            Delete(model.Id);
        }

        #region Protected methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected abstract void Add(T model);

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        protected abstract void Update(T model);

        /// <summary>
        /// Prepares the model for an insert.
        /// </summary>
        /// <param name="model">The model</param>
        protected virtual void PrepareInsert(T model)
        {
            // Prepare id
            model.Id = model.Id != Guid.Empty ? model.Id : Guid.NewGuid();

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
        protected virtual void PrepareUpdate(T model)
        {
            // Prepare last modified date
            if (isModified)
                ((IModified)model).LastModified = DateTime.Now;
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected virtual void AddToCache(T model)
        {
            cache.Set(model.Id.ToString(), model);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected virtual void RemoveFromCache(T model)
        {
            cache.Remove(model.Id.ToString());
        }
        #endregion
    }
}
