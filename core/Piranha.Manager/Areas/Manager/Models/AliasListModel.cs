/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Areas.Manager.Models
{
    public class AliasListModel
    {
        /// <summary>
        /// Gets/sets the available items.
        /// </summary>
        public IEnumerable<Data.Alias> Items { get; set; }

        /// <summary>
        /// Gets/sets the available sites.
        /// </summary>
        public IEnumerable<Data.Site> Sites { get; set; }

        /// <summary>
        /// Gets/sets the id of the currently selected site.
        /// </summary>
        public Guid? SiteId { get; set; }

        /// <summary>
        /// Gets/sets the title of the currently selected site.
        /// </summary>
        public string SiteTitle { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public AliasListModel() {
            Items = new List<Data.Alias>();
            Sites = new List<Data.Site>();
        }

        /// <summary>
        /// Gets the model for the specified site.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="siteId">The optional site id</param>
        /// <returns>The model</returns>
        public static AliasListModel Get(IApi api, Guid? siteId = null) {
            Data.Site site;

            if (!siteId.HasValue) {
                site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            } else {
                site = api.Sites.GetById(siteId.Value);                
            }

            return new AliasListModel() {
                Items = api.Aliases.GetAll(siteId),
                Sites = api.Sites.GetAll(),
                SiteId = siteId,
                SiteTitle = site.Title
            };
        }
    }
}