/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using Piranha.Data;
using System;
using System.IO;
using System.Reflection;

namespace Piranha
{
    /// <summary>
    /// The main application api.
    /// </summary>
    public sealed class Api : IApi, IDisposable
    {
        #region Members
        /// <summary>
        /// The private db context.
        /// </summary>
        private readonly IDb db;

        /// <summary>
        /// The private storage provider.
        /// </summary>
        private readonly IStorage storage;

        /// <summary>
        /// The private model cache.
        /// </summary>
        private ICache cache;
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

        /// <summary>
        /// Gets if the current repository has caching enabled or not.
        /// </summary>
        public bool IsCached {
            get { return cache != null; }
        }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="storage">The current storage</param>
        /// <param name="modelCache">The optional model cache</param>
        public Api(IDb db, IStorage storage, ICache modelCache = null) {
            this.db = db;
            this.storage = storage;

            Setup(modelCache);
        }

        /// <summary>
        /// Disposes the current api.
        /// </summary>
        public void Dispose() {
            db.Dispose();
        }

        #region Private methods
        /// <summary>
        /// Configures the api.
        /// </summary>
        /// <param name="modelCache">The optional model cache</param>
        private void Setup(ICache modelCache = null) {
            cache = modelCache;

            Media = new Repositories.MediaRepository(db, storage, cache);
            Pages = new Repositories.PageRepository(this, db, cache);
            PageTypes = new Repositories.PageTypeRepository(db, cache);
            Params = new Repositories.ParamRepository(db, cache);
            Sites = new Repositories.SiteRepository(this, db, cache);
        }
        #endregion
    }
}
