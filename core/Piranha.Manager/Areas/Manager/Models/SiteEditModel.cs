/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Models;
using System;
using System.Collections.Generic;

namespace Piranha.Areas.Manager.Models
{
    public class SiteEditModel
    {
        public Data.Site Site { get; set; }
        public IEnumerable<SiteType> SiteTypes { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SiteEditModel() 
        {
            Site = new Data.Site();
            SiteTypes = new List<SiteType>();
        }

        public static SiteEditModel Create(IApi api)
        {
            return new SiteEditModel
            {
                SiteTypes = api.SiteTypes.GetAll()
            };
        }

        public static SiteEditModel GetById(IApi api, Guid id) 
        {
            var model = new SiteEditModel
            {
                Site = api.Sites.GetById(id),
                SiteTypes = api.SiteTypes.GetAll()
            };
            return model;
        }

        public bool Save(IApi api) 
        {
            api.Sites.Save(this.Site);

            return true;
        }
    }
}