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
using System.Linq;

namespace Piranha.Repositories
{
    public class SiteRepository : BaseRepository<Site>, ISiteRepository
    {
        private readonly Api api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="connection">The current db connection</param>
        /// <param name="cache">The optional model cache</param>
        public SiteRepository(Api api, IDbConnection connection, ICache cache = null)
            : base(connection, "Piranha_Sites", "Title", modelCache: cache) 
        { 
            this.api = api;
        }

        /// <summary>
        /// Gets the model with the given internal id.
        /// </summary>
        /// <param name="internalId">The unique internal i</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The model</returns>
        public Site GetByInternalId(string internalId, IDbTransaction transaction = null) {
            var id = cache != null ? cache.Get<string>($"SiteId_{internalId}") : null;
            Site model = null;

            if (!string.IsNullOrEmpty(id)) {
                model = GetById(id, transaction);
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

        /// <summary>
        /// Gets the hierachical sitemap structure.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <param name="onlyPublished">If only published items should be included</param>
        /// <param name="transaction">The optional transaction</param>
        /// <returns>The sitemap</returns>
        public Models.Sitemap GetSitemap(string id = null, bool onlyPublished = true, IDbTransaction transaction = null) {
            if (id == null) {
                var site = GetDefault(transaction);

                if (site != null)
                    id = site.Id;
            }

            if (id != null) {
                var sitemap = onlyPublished && cache != null ? cache.Get<Models.Sitemap>($"Sitemap_{id}") : null;

                if (sitemap == null) {
                    var pages = conn.Query<Page>("SELECT [Id], [ParentId], [SortOrder], [Title], [NavigationTitle], [Slug], [IsHidden], [Published], [Created], [LastModified] FROM [Piranha_Pages] WHERE [SiteId]=@Id ORDER BY [ParentId], [SortOrder]",
                        new { Id = id }, transaction: transaction);

                    if (onlyPublished)
                        pages = pages.Where(p => p.Published.HasValue);
                    sitemap = Sort(pages);

                    if (onlyPublished && cache != null)
                        cache.Set($"Sitemap_{id}", sitemap);
                }
                return sitemap;
            }
            return null;
        }

        /// <summary>
        /// Deletes the model with the specified id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="transaction">The optional transaction</param>
        public override void Delete(string id, IDbTransaction transaction = null) {
            // Delete all pages within the site
            var pages = conn.Query<string>($"SELECT [Id] FROM [Piranha_Pages] WHERE [SiteId]=@Id", 
                new { Id = id }, transaction: transaction);

            foreach (var pageId in pages) {
                api.Pages.Delete(pageId, transaction);
            }

            base.Delete(id, transaction);
        }

        /// <summary>
        /// Removes the specified public sitemap from
        /// the cache.
        /// </summary>
        /// <param name="id">The site id</param>
        public void InvalidateSitemap(string id) {
            if (cache != null)
                cache.Remove($"Sitemap_{id}");
        }

        #region Protected methods
        /// <summary>
        /// Adds a new model to the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected override void Add(Site model, IDbTransaction transaction = null) {
            PrepareInsert(model, transaction);

            // Check for title
            if (string.IsNullOrWhiteSpace(model.Title))
                throw new ArgumentException("Title cannot be empty");

            // Ensure InternalId
            if (string.IsNullOrWhiteSpace(model.InternalId))
                model.InternalId = Utils.GenerateInteralId(model.Title);

            if (model.IsDefault) {
                // Make sure no other site is default first
                var def = GetDefault(transaction);

                if (def != null && def.Id != model.Id) {
                    def.IsDefault = false;
                    Update(def, transaction, false);
                }
            } else {
                // Make sure we have a default site
                var count = conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM [{table}] WHERE [IsDefault]=1");
                if (count == 0)
                    model.IsDefault = true;                
            }
            conn.Execute($"INSERT INTO [{table}] ([Id], [InternalId], [Title], [Description], [Hostnames], [IsDefault], [Created], [LastModified]) VALUES (@Id, @InternalId, @Title, @Description, @Hostnames, @IsDefault, @Created, @LastModified)", 
                model, transaction: transaction);
        }

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        protected override void Update(Site model, IDbTransaction transaction = null) {
            Update(model, transaction, true);
        }        

        /// <summary>
        /// Updates the given model in the database.
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="transaction">The optional transaction</param>
        /// <param name="checkDefault">If default site integrity should be validated</param>
        protected void Update(Site model, IDbTransaction transaction = null, bool checkDefault = true) {
            PrepareUpdate(model, transaction);

            if (checkDefault) {
                if (model.IsDefault) {
                    // Make sure no other site is default first
                    var def = GetDefault(transaction);

                    if (def != null && def.Id != model.Id) {
                        def.IsDefault = false;
                        Update(def, transaction, false);
                    }
                } else {
                    // Make sure we have a default site
                    var count = conn.ExecuteScalar<int>($"SELECT COUNT(*) FROM [{table}] WHERE [IsDefault]=1 AND [Id]!=@Id", new {
                        Id = model.Id
                    });
                    if (count == 0)
                        model.IsDefault = true;
                }
            }
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

        #region Private methods
        /// <summary>
        /// Sorts the items.
        /// </summary>
        /// <param name="pages">The full page list</param>
        /// <param name="parentId">The current parent id</param>
        /// <returns>The sitemap</returns>
        private Models.Sitemap Sort(IEnumerable<Page> pages, string parentId = null, int level = 0) {
            var result = new Models.Sitemap();

            foreach (var page in pages.Where(p => p.ParentId == parentId).OrderBy(p => p.SortOrder)) {
                var item = App.Mapper.Map<Page, Models.SitemapItem>(page);

                item.Level = level;
                item.Items = Sort(pages, page.Id, level + 1);

                result.Add(item);
            }
            return result;
        }
        #endregion
    }
}
