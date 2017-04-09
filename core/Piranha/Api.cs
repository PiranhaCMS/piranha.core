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
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Piranha
{
    /// <summary>
    /// The main application api.
    /// </summary>
    public sealed class Api : IDisposable
    {
        #region Members
        /// <summary>
        /// The private db connection.
        /// </summary>
        private readonly IDbConnection conn;

        /// <summary>
        /// The private storage provider.
        /// </summary>
        private readonly IStorage storage;

        /// <summary>
        /// If the api should dispose the connection.
        /// </summary>
        private readonly bool dispose;

        /// <summary>
        /// The private model cache.
        /// </summary>
        private ICache cache;

        /// <summary>
        /// Mutex for database initialization.
        /// </summary>
        private static readonly object mutex = new object();
        #endregion

        #region Properties
        /// <summary>
        /// Gets the media repository.
        /// </summary>
        /// <returns></returns>
        public Repositories.IMediaRepository Media { get; private set; }

        /// <summary>
        /// Gets the page repository.
        /// </summary>
        public Repositories.IPageRepository Pages { get; private set; }

        /// <summary>
        /// Gets the page type repository.
        /// </summary>
        public Repositories.IPageTypeRepository PageTypes { get; private set; }

        /// <summary>
        /// Gets the param repository.
        /// </summary>
        public Repositories.IParamRepository Params { get; private set; }

        /// <summary>
        /// Gets the site repository.
        /// </summary>
        public Repositories.ISiteRepository Sites { get; private set; }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="options">The database builder</param>
        /// <param name="storage">The current storage</param>
        /// <param name="modelCache">The optional model cache</param>
        public Api(Action<DbBuilder> options, IStorage storage, ICache modelCache = null) {
            var config = new DbBuilder();
            options?.Invoke(config);

            if (config.Connection != null) {
                conn = config.Connection;
            } else {
                conn = new SqlConnection(config.ConnectionString);
                dispose = true;
            }

            this.storage = storage;

            Setup(modelCache);

            if (config.Migrate) {
                lock (mutex) {
                    if (!IsCompatible()) {
                        UpdateDatabase();

                        if (config.Seed)
                            Db.Seed(this);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the database is compatible with the current model.
        /// </summary>
        /// <returns>If the database is up to date</returns>
        public bool IsCompatible() {
            return GetMigrationCount() == Db.Migrations.Length;
        }

        /// <summary>
        /// Updates the database to the latest migration.
        /// </summary>
        public void UpdateDatabase() {
            var pos = GetMigrationCount();

            if (pos < Db.Migrations.Length) {
                conn.Open();

                using (var tx = conn.BeginTransaction()) {
                    for (var n = pos; n < Db.Migrations.Length; n++) {
                        conn.Execute(GetMigration(n), transaction: tx);

                        // Add into the migration history
                        conn.Execute("INSERT INTO [Piranha_Migrations] ([Id], [Name], [Created]) VALUES(@Id, @Name, @Created)", new {
                            Id = Guid.NewGuid(),
                            Name = Db.Migrations[n].Name,
                            Created = DateTime.Now
                        }, transaction: tx);
                    }
                    tx.Commit();
                }
                conn.Close();
            }
        }

        /// <summary>
        /// Begins a new transaction.
        /// </summary>
        public IDbTransaction BeginTransaction() {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            return conn.BeginTransaction();
        }

        /// <summary>
        /// Disposes the current api.
        /// </summary>
        public void Dispose() {
            if (dispose) {
                conn.Close();
                conn.Dispose();
            }
        }

        #region Private methods
        /// <summary>
        /// Configures the api.
        /// </summary>
        /// <param name="modelCache">The optional model cache</param>
        private void Setup(ICache modelCache = null) {
            cache = modelCache;

            Media = new Repositories.MediaRepository(conn, storage);
            Pages = new Repositories.PageRepository(this, conn, cache);
            PageTypes = new Repositories.PageTypeRepository(conn, cache);
            Params = new Repositories.ParamRepository(conn, cache);
            Sites = new Repositories.SiteRepository(conn, cache);
        }

        /// <summary>
        /// Gets the current migration count from the database.
        /// </summary>
        /// <returns>The migration count</returns>
        private int GetMigrationCount() {
            try {
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM Piranha_Migrations");
            } catch { }

            return 0;
        }

        /// <summary>
        /// Gets the embedded migration at the specific position.
        /// </summary>
        /// <param name="index">The migration</param>
        /// <returns>The migration script</returns>
        private string GetMigration(int index) {
            var assembly = typeof(Api).GetTypeInfo().Assembly;

            using (var reader = new StreamReader(assembly.GetManifestResourceStream(Db.Migrations[index].Script))) {
                return reader.ReadToEnd();
            }
        }
        #endregion
    }
}
