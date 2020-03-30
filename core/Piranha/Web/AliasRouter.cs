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
using System.Threading.Tasks;

namespace Piranha.Web
{
    public class AliasRouter
    {
        /// <summary>
        /// Invokes the router.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="url">The requested url</param>
        /// <param name="siteId">The requested site id</param>
        /// <returns>The piranha response, null if no matching page was found</returns>
        public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
        {
            if (!String.IsNullOrWhiteSpace(url) && url.Length > 1)
            {
                // Check if we can find an alias with the requested url
                var alias = await api.Aliases.GetByAliasUrlAsync(url, siteId)
                    .ConfigureAwait(false);

                if (alias != null)
                {
                    return new RouteResponse
                    {
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
