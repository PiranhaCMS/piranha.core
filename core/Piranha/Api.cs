/*
 * Copyright (c) 2017-2018 Håkan Edling
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
        /// <summary>
        /// The private db context.
        /// </summary>
        private readonly IDb db;

        /// <summary>
        /// The private storage provider.
        /// </summary>
        private readonly IStorage storage;

        /// <summary>
        /// The private service provider.
        /// </summary>
        private readonly IServiceProvider services;

        /// <summary>
        /// The private model cache.
        /// </summary>
        private ICache cache;

        /// <summary>
        /// Gets/sets the alias repository.
        /// </summary>
        public Repositories.IAliasRepository Aliases { get; private set; }

        /// <summary>
        /// Gets/sets the archive repository.
        /// </summary>
        public Repositories.IArchiveRepository Archives { get; private set; }

        /// <summary>
        /// Gets the category repository.
        /// </summary>
        public Repositories.ICategoryRepository Categories { get; private set; }

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
        /// Gets the post repository.
        /// </summary>
        public Repositories.IPostRepository Posts { get; private set; }

        /// <summary>
        /// Gets the post type repository.
        /// </summary>
        public Repositories.IPostTypeRepository PostTypes { get; private set; }

        /// <summary>
        /// Gets the site repository.
        /// </summary>
        public Repositories.ISiteRepository Sites { get; private set; }

        /// <summary>
        /// Gets the tag repository.
        /// </summary>
        public Repositories.ITagRepository Tags { get; private set; }

        /// <summary>
        /// Gets if the current repository has caching enabled or not.
        /// </summary>
        public bool IsCached {
            get { return cache != null; }
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="services">The service provider</param>
        /// <param name="db">The current db context</param>
        /// <param name="storage">The current storage</param>
        /// <param name="modelCache">The optional model cache</param>
        /// <param name="imageProcessor">The optional image processor</param>
        public Api(IServiceProvider services, IDb db, IStorage storage = null, ICache modelCache = null, IImageProcessor imageProcessor = null) {
            this.db = db;
            this.storage = storage;
            this.services = services;

            Setup(modelCache, imageProcessor);
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
        /// <param name="imageProcessor">The optional image processor</param>
        private void Setup(ICache modelCache = null, IImageProcessor imageProcessor = null) {
            cache = modelCache;
            
            var cacheLevel = (int)App.CacheLevel;

            Aliases = new Repositories.AliasRepository(this, db, cacheLevel > 2 ? cache : null);
            Archives = new Repositories.ArchiveRepository(this, db);
            Categories = new Repositories.CategoryRepository(this, db, cacheLevel > 2 ? cache : null);
            Media = new Repositories.MediaRepository(this, db, storage, cacheLevel > 2 ? cache : null, imageProcessor);
            Pages = new Repositories.PageRepository(this, db, services, cacheLevel > 2 ? cache : null);
            PageTypes = new Repositories.PageTypeRepository(db, cacheLevel > 1 ? cache : null);
            Params = new Repositories.ParamRepository(db, cacheLevel > 0 ? cache : null);
            Posts = new Repositories.PostRepository(this, db, services, cacheLevel > 2 ? cache : null);
            PostTypes = new Repositories.PostTypeRepository(db, cacheLevel > 1 ? cache : null);
            Sites = new Repositories.SiteRepository(this, db, cacheLevel > 0 ? cache : null);
            Tags = new Repositories.TagRepository(db, cacheLevel > 2 ? cache : null);
        }
        #endregion
    }
}
