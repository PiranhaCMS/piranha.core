/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Linq;
using System.Threading.Tasks;
using Piranha.Models;
using Piranha.Manager.Models;

namespace Piranha.Manager.Services
{
    public class AliasService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public AliasService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the list model for the specified site. If no site is
        /// specified the default site is used.
        /// </summary>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The list model</returns>
        public async Task<AliasListModel> GetList(Guid? siteId = null)
        {
            Site site = null;

            // Ensure that we have a site id
            if (!siteId.HasValue)
            {
                site = await _api.Sites.GetDefaultAsync();
                siteId = site.Id;
            }

            if (site == null)
            {
                site = await _api.Sites.GetByIdAsync(siteId.Value);
            }

            var model = new AliasListModel
            {
                SiteId = siteId.Value,
                SiteTitle = site.Title
            };

            // Get all available sites
            var sites = await _api.Sites.GetAllAsync();
            model.Sites = sites.Select(s => new AliasListModel.SiteItem
            {
                Id = s.Id,
                Title = s.Title
            }).ToList();

            // Get all available aliases for the current site
            var aliases = await _api.Aliases.GetAllAsync(siteId.Value);
            model.Items = aliases.Select(a => new AliasListModel.ListItem
            {
                Id = a.Id,
                SiteId = a.SiteId,
                AliasUrl = a.AliasUrl,
                RedirectUrl = a.RedirectUrl,
                IsPermanent = a.Type == RedirectType.Permanent
            }).ToList();

            return model;
        }

        /// <summary>
        /// Saves the given alias.
        /// </summary>
        /// <param name="model">The alias</param>
        public async Task Save(AliasListModel.ListItem model)
        {
            await _api.Aliases.SaveAsync(new Alias
            {
                Id = model.Id.HasValue ? model.Id.Value : Guid.NewGuid(),
                SiteId = model.SiteId,
                AliasUrl = model.AliasUrl,
                RedirectUrl = model.RedirectUrl,
                Type = model.IsPermanent ? RedirectType.Permanent : RedirectType.Temporary
            });
        }

        /// <summary>
        /// Deletes the alias with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The deleted alias</returns>
        public async Task<AliasListModel.ListItem> Delete(Guid id)
        {
            var alias = await _api.Aliases.GetByIdAsync(id);

            if (alias != null)
            {
                await _api.Aliases.DeleteAsync(alias);

                return new AliasListModel.ListItem
                {
                    Id = alias.Id,
                    SiteId = alias.SiteId,
                    AliasUrl = alias.AliasUrl,
                    RedirectUrl = alias.RedirectUrl,
                    IsPermanent = alias.Type == RedirectType.Permanent
                };
            }
            return null;
        }
    }
}