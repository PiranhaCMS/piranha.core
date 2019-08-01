/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;
using Piranha.Manager.Models.Content;
using Piranha.Services;

namespace Piranha.Manager.Services
{
    public class SiteService
    {
        private readonly IApi _api;
        private readonly IContentFactory _factory;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SiteService(IApi api, IContentFactory factory)
        {
            _api = api;
            _factory = factory;
        }

        /// <summary>
        /// Gets the edit model for the site with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The edit model</returns>
        public async Task<SiteEditModel> GetById(Guid id)
        {
            var site = await _api.Sites.GetByIdAsync(id);

            if (site != null)
            {
                return Transform(site);
            }
            return null;
        }

        /// <summary>
        /// Creates a new site edit model.
        /// </summary>
        /// <returns>The edit model</returns>
        public SiteEditModel Create()
        {
            return new SiteEditModel
            {
                Id = Guid.NewGuid()
            };
        }

        public async Task Save(SiteEditModel model)
        {
            var site = await _api.Sites.GetByIdAsync(model.Id);

            if (site == null)
            {
                site = new Site
                {
                    Id = model.Id
                };
            }
            site.SiteTypeId = model.TypeId;
            site.Title = model.Title;
            site.InternalId = model.InternalId;
            site.Culture = model.Culture;
            site.Hostnames = model.Hostnames;
            site.Description = model.Description;
            site.IsDefault = model.IsDefault;

            await _api.Sites.SaveAsync(site);
        }

        /// <summary>
        /// Deletes the site with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        public Task Delete(Guid id)
        {
            return _api.Sites.DeleteAsync(id);
        }

        private SiteEditModel Transform(Site site)
        {
            return new SiteEditModel
            {
                Id = site.Id,
                TypeId = site.SiteTypeId,
                Title = site.Title,
                InternalId = site.InternalId,
                Culture = site.Culture,
                Description = site.Description,
                Hostnames = site.Hostnames,
                IsDefault = site.IsDefault,
                SiteTypes = App.SiteTypes.Select(t => new ContentTypeModel
                {
                    Id = t.Id,
                    Title = t.Title
                }).ToList()
            };
        }
    }
}