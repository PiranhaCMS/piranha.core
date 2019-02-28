/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Piranha.Services;
using Piranha.Repositories;
using System;

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
        private readonly IDb _db;

        /// <summary>
        /// The private storage provider.
        /// </summary>
        private readonly IStorage _storage;

        /// <summary>
        /// The private model cache.
        /// </summary>
        private ICache _cache;

        /// <summary>
        /// Gets/sets the alias service.
        /// </summary>
        public AliasService Aliases { get; private set; }

        /// <summary>
        /// Gets/sets the archive service.
        /// </summary>
        public ArchiveService Archives { get; private set; }

        /// <summary>
        /// Gets the media service.
        /// </summary>
        /// <returns></returns>
        public MediaService Media { get; private set; }

        /// <summary>
        /// Gets the page service.
        /// </summary>
        public PageService Pages { get; private set; }

        /// <summary>
        /// Gets the page type service.
        /// </summary>
        public PageTypeService PageTypes { get; private set; }

        /// <summary>
        /// Gets the param service.
        /// </summary>
        public ParamService Params { get; private set; }

        /// <summary>
        /// Gets the post service.
        /// </summary>
        public PostService Posts { get; private set; }

        /// <summary>
        /// Gets the post type service.
        /// </summary>
        public PostTypeService PostTypes { get; private set; }

        /// <summary>
        /// Gets the site service.
        /// </summary>
        public SiteService Sites { get; private set; }

        /// <summary>
        /// Gets the site type service.
        /// </summary>
        public SiteTypeService SiteTypes { get; private set; }

        /// <summary>
        /// Gets if the current repository has caching enabled or not.
        /// </summary>
        public bool IsCached => _cache != null;

        /// <summary>
        /// Creates a new api from the currently registered
        /// repositories.
        /// </summary>
        public Api(
            IContentFactory contentFactory,
            IAliasRepository aliasRepository,
            IArchiveRepository archiveRepository,
            IMediaRepository mediaRepository,
            IPageRepository pageRepository,
            IPageTypeRepository pageTypeRepository,
            IParamRepository paramRepository,
            IPostRepository postRepository,
            IPostTypeRepository postTypeRepository,
            ISiteRepository siteRepository,
            ISiteTypeRepository siteTypeRepository,
            ICache cache = null,
            IStorage storage = null,
            IImageProcessor processor = null)
        {
            var cacheLevel = (int)App.CacheLevel;

            // Old repositories

            // Create services without dependecies
            PageTypes = new PageTypeService(pageTypeRepository, cache);
            Params = new ParamService(paramRepository, cache);
            PostTypes = new PostTypeService(postTypeRepository, cache);
            Sites = new SiteService(siteRepository, contentFactory, cache);
            SiteTypes = new SiteTypeService(siteTypeRepository, cache);

            // Create services with dependencies
            Aliases = new AliasService(aliasRepository, Sites, cache);
            Media = new MediaService(mediaRepository, Params, storage, processor, cache);
            Pages = new PageService(pageRepository, contentFactory, Sites, Params, cache);
            Posts = new PostService(postRepository, contentFactory, Sites, Pages, cache);
            Archives = new ArchiveService(archiveRepository, Pages, Params, Posts);
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="db">The current db context</param>
        /// <param name="factory">The content service factory</param>
        /// <param name="storage">The current storage</param>
        /// <param name="modelCache">The optional model cache</param>
        /// <param name="imageProcessor">The optional image processor</param>
        public Api(IDb db, IContentFactory contentFactory, IContentServiceFactory factory, IStorage storage = null, ICache modelCache = null, IImageProcessor imageProcessor = null)
        {
            _db = db;
            _storage = storage;

            Setup(contentFactory, factory, modelCache, imageProcessor);
        }

        /// <summary>
        /// Disposes the current api.
        /// </summary>
        public void Dispose()
        {
            _db.Dispose();
        }

        /// <summary>
        /// Configures the api.
        /// </summary>
        /// <param name="modelCache">The optional model cache</param>
        /// <param name="imageProcessor">The optional image processor</param>
        private void Setup(IContentFactory contentFactory, IContentServiceFactory factory, ICache modelCache = null, IImageProcessor imageProcessor = null)
        {
            _cache = modelCache;

            var cacheLevel = (int)App.CacheLevel;

            // Create services without dependecies
            PageTypes = new PageTypeService(new PageTypeRepository(_db), _cache);
            Params = new ParamService(new ParamRepository(_db), _cache);
            PostTypes = new PostTypeService(new PostTypeRepository(_db), _cache);
            Sites = new SiteService(new SiteRepository(_db, factory), contentFactory, _cache);
            SiteTypes = new SiteTypeService(new SiteTypeRepository(_db), _cache);

            // Create services with dependencies
            Aliases = new AliasService(new AliasRepository(_db), Sites, _cache);
            Media = new MediaService(new MediaRepository(_db), Params, _storage, imageProcessor, _cache);
            Pages = new PageService(new PageRepository(_db, factory), contentFactory, Sites, Params, _cache);
            Posts = new PostService(new PostRepository(_db, factory), contentFactory, Sites, Pages, _cache);
            Archives = new ArchiveService(new ArchiveRepository(_db), Pages, Params, Posts);
        }
    }
}
