/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Piranha.Manager
{
    public static class MenuExtensions
    {
        /// <summary>
        /// Gets the applicable menu structure for the given user.
        /// </summary>
        /// <param name="items">The menu items to filter</param>
        /// <param name="user">The user</param>
        /// <param name="auth">The authorization service</param>
        /// <returns></returns>
        public static async Task<MenuItemList> GetForUser(this MenuItemList items, ClaimsPrincipal user, IAuthorizationService auth)
        {
            var result = new MenuItemList();

            foreach (var group in items)
            {
                if (string.IsNullOrWhiteSpace(group.Policy) || (await auth.AuthorizeAsync(user, group.Policy)).Succeeded)
                {
                    var resultGroup = new MenuItem
                    {
                        InternalId = group.InternalId,
                        Name = group.Name,
                        Css = group.Css
                    };

                    foreach (var item in group.Items)
                    {
                        // Add the item if the given user has access to it
                        if (string.IsNullOrWhiteSpace(item.Policy) || (await auth.AuthorizeAsync(user, item.Policy)).Succeeded)
                        {
                            resultGroup.Items.Add(new MenuItem
                            {
                                InternalId = item.InternalId,
                                Name = item.Name,
                                Route = item.Route,
                                Policy = item.Policy,
                                Css = item.Css
                            });
                        }
                    }

                    // Only add menu groups that contains items
                    if (resultGroup.Items.Count > 0)
                    {
                        result.Add(resultGroup);
                    }
                }
            }
            return result;
        }
    }
}