/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using Piranha.Models;
using Piranha.Services;

namespace Piranha.Areas.Manager.Models
{
    public class SiteEditModel
    {
        public Site Site { get; set; } = new Site();
        public IEnumerable<SiteType> SiteTypes { get; set; } = new List<SiteType>();

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
            api.Sites.Save(Site);

            return true;
        }
    }
}