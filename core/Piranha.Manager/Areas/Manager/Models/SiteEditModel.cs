/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Areas.Manager.Models
{
    public class SiteEditModel
    {
        public Data.Site Site { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public SiteEditModel() {
            Site = new Data.Site();
        }

        public static SiteEditModel GetById(Api api, string id) {
            var model = new SiteEditModel() {
                Site = api.Sites.GetById(id)
            };
            return model;
        }

        public bool Save(Api api) {
            api.Sites.Save(this.Site);

            return true;
        }
    }
}