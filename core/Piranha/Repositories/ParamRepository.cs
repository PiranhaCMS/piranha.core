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
using System.Data;

namespace Piranha.Repositories
{
    public class ParamRepository : BaseRepository<Param>, IParamRepository
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connection">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        public ParamRepository(IDbConnection connection, ICache cache = null) 
            : base(connection, "Piranha_Params", "Key", modelCache: cache) { }

        /// <summary>
        /// Gets the model with the given key.
        /// </summary>
        /// <param name="key">The unique key</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The model</returns>
        public Param GetByKey(string key, IDbTransaction transaction = null) {
            var id = cache != null ? cache.Get<string>($"ParamKey_{key}") : null;
            Param model = null;

            if (!string.IsNullOrEmpty(id)) {
                model = GetById(id, transaction);
            } else {
                model = conn.QueryFirstOrDefault<Param>($"SELECT * FROM [{table}] WHERE [Key]=@Key",
                    new { Key = key }, transaction: transaction);

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
        protected override void Add(Param model, IDbTransaction transaction = null) {
            PrepareInsert(model, transaction);

            conn.Execute($"INSERT INTO [{table}] ([Id], [Key], [Value], [Description], [Created], [LastModified]) VALUES (@Id, @Key, @Value, @Description, @Created, @LastModified)", 
                model, transaction: transaction);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected override void Update(Param model, IDbTransaction transaction = null) {
            PrepareUpdate(model, transaction);

            conn.Execute($"UPDATE [{table}] SET [Key]=@Key, [Value]=@Value, [Description]=@Description, [LastModified]=@LastModified WHERE [Id]=@Id", 
                model, transaction: transaction);
        }

        /// <summary>
        /// Adds the given model to cache.
        /// </summary>
        /// <param name="model">The model</param>
        protected override void AddToCache(Param model) {
            cache.Set(model.Id.ToString(), model);
            cache.Set($"ParamKey_{model.Key}", model.Id);
        }
        #endregion
    }
}
