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
using Piranha.Repositories;

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
        /// Gets/sets the alias repository.
        /// </summary>
        public IAliasRepository Aliases { get; private set; }

        /// <summary>
        /// Gets/sets the archive repository.
        /// </summary>
        public IArchiveRepository Archives { get; private set; }

        /// <summary>
        /// Gets the category repository.
        /// </summary>
        public ICategoryRepository Categories { get; private set; }

        /// <summary>
        /// Gets the media repository.
        /// </summary>
        /// <returns></returns>
        public IMediaRepository Media { get; private set; }

        /// <summary>
        /// Gets the page repository.
        /// </summary>
        public IPageRepository Pages { get; private set; }

        /// <summary>
        /// Gets the page type repository.
        /// </summary>
        public IPageTypeRepository PageTypes { get; private set; }

        /// <summary>
        /// Gets the param repository.
        /// </summary>
        public IParamRepository Params { get; private set; }

        /// <summary>
        /// Gets the post repository.
        /// </summary>
        public IPostRepository Posts { get; private set; }

        /// <summary>
        /// Gets the post type repository.
        /// </summary>
        public IPostTypeRepository PostTypes { get; private set; }

        /// <summary>
        /// Gets the site repository.
        /// </summary>
        public ISiteRepository Sites { get; private set; }

        /// <summary>
        /// Gets the tag repository.
        /// </summary>
        public ITagRepository Tags { get; private set; }

        /// <summary>
        /// Gets if the current repository has caching enabled or not.
        /// </summary>
        public bool IsCached
        {
            get { return cache != null; }
        }
        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="storage">The current storage</param>
        /// <param name="modelCache">The optional model cache</param>
        /// <param name="imageProcessor">The optional image processor</param>
        public Api(IDb db, IStorage storage, ICache modelCache = null, IImageProcessor imageProcessor = null)
        {
            this.db = db;
            this.storage = storage;

            Setup(modelCache, imageProcessor);
        }

        /// <summary>
        /// Disposes the current api.
        /// </summary>
        public void Dispose()
        {
            db.Dispose();
        }

        #region Private methods
        /// <summary>
        /// Configures the api.
        /// </summary>
        /// <param name="modelCache">The optional model cache</param>
        /// <param name="imageProcessor">The optional image processor</param>
        private void Setup(ICache modelCache = null, IImageProcessor imageProcessor = null)
        {
            cache = modelCache;

            Aliases = new AliasRepository(this, db, cache);
            Archives = new ArchiveRepository(this, db);
            Categories = new CategoryRepository(this, db, cache);
            Media = new MediaRepository(this, db, storage, cache, imageProcessor);
            Pages = new PageRepository(this, db, cache);
            PageTypes = new PageTypeRepository(db, cache);
            Params = new ParamRepository(db, cache);
            Posts = new PostRepository(this, db, cache);
            PostTypes = new PostTypeRepository(db, cache);
            Sites = new SiteRepository(this, db, cache);
            Tags = new TagRepository(db, cache);
        }
        #endregion
    }
}
