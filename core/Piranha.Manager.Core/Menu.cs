﻿/*
 * Copyright (c) 2016-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager
{
    /// <summary>
    /// Static class for defining the manager menu.
    /// </summary>
    public static class Menu
    {
        /// <summary>
        /// The basic manager menu.
        /// </summary>
        public static MenuItemList Items = new MenuItemList
        {
            new MenuItem
            {
                InternalId = "Content", Name = "Content", Css = "fas fa-pencil-alt", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Pages", Name = "Pages", Route = "~/manager/pages", Policy = Permission.Pages, Css = "fas fa-copy"
                    },
                    new MenuItem
                    {
                        InternalId = "Media", Name = "Media", Route = "~/manager/media", Policy = Permission.Media, Css = "fas fa-images"
                    }
                }
            },
            new MenuItem
            {
                InternalId = "Settings", Name = "Settings", Css = "fas fa-wrench", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Aliases", Name = "Aliases", Route = "~/manager/aliases", Policy = Permission.Aliases, Css = "fas fa-random"
                    },
                    new MenuItem
                    {
                        InternalId = "Sites", Name = "Sites", Route = "~/manager/aliases", Policy = Permission.Sites, Css = "fas fa-globe"
                    }
                }
            },
            new MenuItem
            {
                InternalId = "System", Name = "System", Css = "fas fa-cog", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Config", Name = "Config", Route = "~/manager/config", Policy = Permission.Config, Css = "fas fa-cogs"
                    },
                    new MenuItem
                    {
                        InternalId = "Modules", Name = "Modules", Route = "~/manager/modules", Policy = Permission.Config, Css = "fas fa-code-branch"
                    }
                }
            }
        };
    }
}