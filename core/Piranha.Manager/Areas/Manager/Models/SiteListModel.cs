/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using System.Linq;

namespace Piranha.Areas.Manager.Models
{
    public class SiteListModel
    {
        public IList<Data.Site> Sites { get; set; }

        public SiteListModel() {
            Sites = new List<Data.Site>();
        }

        public static SiteListModel Get(IApi api) {
            var model = new SiteListModel() {
                Sites = api.Sites.GetAll().ToList()
            };
            return model;
        }
    }
}