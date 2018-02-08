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

namespace Piranha.Web
{
    public class AliasRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="hostname">The optional hostname</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static IRouteResponse Invoke(IApi api, string url, string hostname) {
            Data.Site site = null;
            
            if (!string.IsNullOrWhiteSpace(hostname))
                site = api.Sites.GetByHostname(hostname);
            if (site == null)
                site = api.Sites.GetDefault();

            if (!String.IsNullOrWhiteSpace(url) && url.Length > 1) {
                // Check if we can find an alias with the requested url
                var alias = api.Aliases.GetByAliasUrl(url, site.Id);

                if (alias != null) {
                    return new RouteResponse() {
                        IsPublished = true,
                        RedirectUrl = alias.RedirectUrl,
                        RedirectType = alias.Type
                    };
                }
            }
            return null;
        }
    }
}
