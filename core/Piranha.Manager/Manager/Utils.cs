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
using System.Linq;

namespace Piranha.Manager
{
    public static class Utils
    {
        /// <summary>
        /// Creates a new alias after saving a page or post.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="siteId">The site id</param>
        /// <param name="alias">The alias url</param>
        /// <param name="redirect">The redirect url</param>
        public static void CreateAlias(IApi api, Guid siteId, string alias, string redirect) {
            // First check if there's an alias pointing to the old url
            var aliases = api.Aliases.GetByRedirectUrl($"/{alias}", siteId);

            if (aliases.Count() > 0) {
                foreach (var model in aliases) {
                    // Check for circular references
                    if (model.AliasUrl == $"/{redirect}") {
                        api.Aliases.Delete(model);
                    } else {
                        // Update the existing alias
                        model.RedirectUrl = $"/{redirect}";
                        api.Aliases.Save(model);
                    }
                }
            } 
            
            // Check for an existing alias
            var aliasModel = api.Aliases.GetByAliasUrl($"/{alias}", siteId);
            if (aliasModel != null) {
                aliasModel.RedirectUrl = redirect;
                api.Aliases.Save(aliasModel);
            } else {
                // Let's create a new alias
                api.Aliases.Save(new Data.Alias() {
                    SiteId = siteId,
                    AliasUrl = alias,
                    RedirectUrl = redirect,
                    Type = Piranha.Models.RedirectType.Permanent
                });
            }
        }        
    }
}