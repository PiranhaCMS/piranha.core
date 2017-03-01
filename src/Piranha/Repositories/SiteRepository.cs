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
using System.Data;

namespace Piranha.Repositories
{
    public class SiteRepository : BaseRepository<Site>, ISiteRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connection">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        public SiteRepository(IDbConnection connection, ICache cache = null)
            : base(connection, "Piranha_Sites", "Title", modelCache: cache) { }

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The model</returns>
        public Site GetByInternalId(string internalId, IDbTransaction transaction = null) {
            Guid? id = cache != null ? cache.Get<Guid>($"SiteId_{internalId}") : (Guid?)null;
            Site model = null;

            if (id.HasValue) {
                model = GetById(id.Value, transaction);
            } else {
                model = conn.QueryFirstOrDefault<Site>($"SELECT * FROM [{table}] WHERE [InternalId]=@InternalId",
                    new { InternalId = internalId }, transaction: transaction);

                if (cache != null && model != null)
                    AddToCache(model);
            }
            return model;
        }

        /// <summary>
        /// Gets the default side.
        /// </summary>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The modell, or NULL if it doesnt exist</returns>
        public Site GetDefault(IDbTransaction transaction = null) {
            Site model = cache != null ? cache.Get<Site>($"Site_{Guid.Empty}") : null;

            if (model == null) {
                model = conn.QueryFirstOrDefault<Site>($"SELECT * FROM [{table}] WHERE [IsDefault]=1",
                    transaction: transaction);

                if (cache != null && model != null)
                    AddToCache(model);
            }
            return model;
        }

        #region Protected methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected override void Add(Site model, IDbTransaction transaction = null) {
            PrepareInsert(model, transaction);

            conn.Execute($"INSERT INTO [{table}] ([Id], [InternalId], [Title], [Description], [Hostnames], [IsDefault], [Created], [LastModified]) VALUES (@Id, @InternalId, @Title, @Description, @Hostnames, @IsDefault, @Created, @LastModified)", 
                model, transaction: transaction);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected override void Update(Site model, IDbTransaction transaction = null) {
            PrepareUpdate(model, transaction);

            conn.Execute($"UPDATE [{table}] SET [InternalId]=@InternalId, [Title]=@Title, [Description]=@Description, [Hostnames]=@Hostnames, [IsDefault]=@IsDefault, [LastModified]=@LastModified WHERE [Id]=@Id", 
                model, transaction: transaction);
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Site model) {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"SiteId_{model.InternalId}", model.Id);
            if (model.IsDefault)
                cache.Set($"Site_{Guid.Empty}", model);
        }

        /// <summary>
        /// Removes the given model from cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void RemoveFromCache(Site model) {
            cache.Remove($"SiteId_{model.InternalId}");
            if (model.IsDefault)
                cache.Remove($"Site_{Guid.Empty}");

            base.RemoveFromCache(model);
        }
        #endregion
    }
}
