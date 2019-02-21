/*
 * Copyright (c) 2017-2018 Håkan Edling
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
        /// Gets the category service.
        /// </summary>
        public CategoryService Categories { get; private set; }

        /// <summary>
        /// Gets the media repository.
        /// </summary>
        /// <returns></returns>
        public IMediaRepository Media { get; private set; }

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
        /// Gets the post repository.
        /// </summary>
        public IPostRepository Posts { get; private set; }

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
        /// Gets the tag service.
        /// </summary>
        public TagService Tags { get; private set; }

        /// <summary>
        /// Gets if the current repository has caching enabled or not.
        /// </summary>
        public bool IsCached => _cache != null;

        /// <summary>
        /// Creates a new api from the currently registered
        /// repositories.
        /// </summary>
        public Api(
            IAliasRepository aliasRepository,
            IArchiveRepository archiveRepository,
            ICategoryRepository categoryRepository,
            IMediaRepository mediaRepository,
            IPageRepository pageRepository,
            IPageTypeRepository pageTypeRepository,
            IParamRepository paramRepository,
            IPostRepository postRepository,
            IPostTypeRepository postTypeRepository,
            ISiteRepository siteRepository,
            ISiteTypeRepository siteTypeRepository,
            ITagRepository tagRepository,
            ICache cache = null)
        {
            var cacheLevel = (int)App.CacheLevel;

            // Old repositories
            Media = mediaRepository;
            Posts = postRepository;

            // Create services without dependecies
            Categories = new CategoryService(categoryRepository, cacheLevel > 2 ? _cache : null);
            PageTypes = new PageTypeService(pageTypeRepository, cacheLevel > 0 ? _cache : null);
            Params = new ParamService(paramRepository, cacheLevel > 0 ? _cache : null);
            PostTypes = new PostTypeService(postTypeRepository, cacheLevel > 0 ? _cache : null);
            Sites = new SiteService(siteRepository, cacheLevel > 0 ? _cache : null);
            SiteTypes = new SiteTypeService(siteTypeRepository, cacheLevel > 0 ? _cache : null);
            Tags = new TagService(tagRepository, cacheLevel > 2 ? _cache : null);

            // Create services with dependencies
            Aliases = new AliasService(aliasRepository, Sites, cacheLevel > 2 ? _cache : null);
            Pages = new PageService(pageRepository, Sites, Params, cacheLevel > 2 ? _cache : null);
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
        public Api(IDb db, IContentServiceFactory factory, IStorage storage = null, ICache modelCache = null, IImageProcessor imageProcessor = null)
        {
            _db = db;
            _storage = storage;

            Setup(factory, modelCache, imageProcessor);
        }

        /// <summary>
        /// Disposes the current api.
        /// </summary>
        public void Dispose()
        {
            _db.Dispose();
        }

        #region Private methods
        /// <summary>
        /// Configures the api.
        /// </summary>
        /// <param name="modelCache">The optional model cache</param>
        /// <param name="imageProcessor">The optional image processor</param>
        private void Setup(IContentServiceFactory factory, ICache modelCache = null, IImageProcessor imageProcessor = null)
        {
            _cache = modelCache;

            var cacheLevel = (int)App.CacheLevel;

            // Old repositories
            Media = new MediaRepository(this, _db, _storage, cacheLevel > 2 ? _cache : null, imageProcessor);
            Posts = new PostRepository(this, _db, factory, cacheLevel > 2 ? _cache : null);

            // Create services without dependecies
            Categories = new CategoryService(new CategoryRepository(_db), cacheLevel > 2 ? _cache : null);
            PageTypes = new PageTypeService(new PageTypeRepository(_db), cacheLevel > 0 ? _cache : null);
            Params = new ParamService(new ParamRepository(_db), cacheLevel > 0 ? _cache : null);
            PostTypes = new PostTypeService(new PostTypeRepository(_db), cacheLevel > 0 ? _cache : null);
            Sites = new SiteService(new SiteRepository(_db, factory), cacheLevel > 0 ? _cache : null);
            SiteTypes = new SiteTypeService(new SiteTypeRepository(_db), cacheLevel > 0 ? _cache : null);
            Tags = new TagService(new TagRepository(_db), cacheLevel > 2 ? _cache : null);

            // Create services with dependencies
            Aliases = new AliasService(new AliasRepository(_db), Sites, cacheLevel > 2 ? _cache : null);
            Pages = new PageService(new PageRepository(_db, factory), Sites, Params, cacheLevel > 2 ? _cache : null);
            Archives = new ArchiveService(new ArchiveRepository(_db), Pages, Params, Posts);
        }
        #endregion
    }
}
