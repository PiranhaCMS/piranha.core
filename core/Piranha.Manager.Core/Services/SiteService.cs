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
            site.Title = model.Title;
            site.InternalId = model.InternalId;
            site.Culture = model.Culture;
            site.Hostnames = model.Hostnames;
            site.Description = model.Description;
            site.IsDefault = model.IsDefault;

            await _api.Sites.SaveAsync(site);
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
                IsDefault = site.IsDefault
            };
        }
    }
}