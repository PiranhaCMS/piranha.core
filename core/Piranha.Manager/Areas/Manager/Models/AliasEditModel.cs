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

namespace Piranha.Areas.Manager.Models
{
    public class AliasEditModel
    {
        public IEnumerable<Data.Alias> Items { get; set; }
        public IEnumerable<Data.Site> Sites { get; set; }
        public Guid? SiteId { get; set; }
        public string SiteTitle { get; set; }

        public AliasEditModel() {
            Items = new List<Data.Alias>();
            Sites = new List<Data.Site>();
        }

        public static AliasEditModel Get(IApi api, Guid? siteId = null) {
            Data.Site site;

            if (!siteId.HasValue) {
                site = api.Sites.GetDefault();
                if (site != null)
                    siteId = site.Id;
            } else {
                site = api.Sites.GetById(siteId.Value);                
            }

            var model = new AliasEditModel() {
                Items = api.Aliases.GetAll(siteId),
                Sites = api.Sites.GetAll(),
                SiteId = siteId,
                SiteTitle = site.Title
            };
            return model;
        }
    }
}