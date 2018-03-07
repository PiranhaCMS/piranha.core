﻿/*
 * Copyright (c) 2016-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using System.Linq;

namespace Piranha.Manager
{
    /// <summary>
    /// Static class for defining the manager menu.
    /// </summary>
    public static class Menu
    {
        #region Inner classes
        /// <summary>
        /// An item in the manager menu.
        /// </summary>
        public class MenuItem
        {
            #region Properties
            /// <summary>
            /// Gets/sets the internal id.
            /// </summary>
            public string InternalId { get; set; }

            /// <summary>
            /// Gets/sets the display name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets/sets the optional css class.
            /// </summary>
            public string Css { get; set; }

            /// <summary>
            /// Gets/sets the manager controller.
            /// </summary>
            public string Controller { get; set; }

            /// <summary>
            /// Gets/sets the default action to invoke.
            /// </summary>
            public string Action { get; set; }

            /// <summary>
            /// Gets/sets the policy needed to see this item.
            /// </summary>
            public string Policy { get; set; }

            /// <summary>
            /// Gets/sets the optional menu item params.
            /// </summary>
            public object Params { get; set; }

            /// <summary>
            /// Gets/sets the available items.
            /// </summary>
            public MenuItemList Items { get; set; }
            #endregion

            /// <summary>
            /// Default constructor.
            /// </summary>
            public MenuItem()
            {
                Items = new MenuItemList();
            }
        }

        public class MenuItemList : List<MenuItem>
        {
            /// <summary>
            /// Gets the menu item with the given internal id.
            /// </summary>
            public MenuItem this[string internalId]
            {
                get
                {
                    return this.FirstOrDefault(i => i.InternalId == internalId);
                }
            }
        }
        #endregion

        /// <summary>
        /// The basic manager menu.
        /// </summary>
        public static MenuItemList Items = new MenuItemList
        {
            new MenuItem
            {
                InternalId = "Content", Name = "Content", Css = "glyphicon glyphicon-pencil", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Pages", Name = "Pages", Controller = "Page", Action = "List", Policy = Permission.Pages, Css = "glyphicon glyphicon-duplicate"
                    },
                    new MenuItem
                    {
                        InternalId = "Media", Name = "Media", Controller = "Media", Action = "List", Policy = Permission.Media, Css = "glyphicon glyphicon-picture"
                    }
                }
            },
            new MenuItem
            {
                InternalId = "Settings", Name = "Settings", Css = "glyphicon glyphicon-wrench", Items = new MenuItemList {
                    new MenuItem
                    {
                        InternalId = "Aliases", Name = "Aliases", Controller = "Alias", Action = "List", Policy = Permission.Aliases, Css = "glyphicon glyphicon-random"
                    },
                    new MenuItem
                    {
                        InternalId = "Sites", Name = "Sites", Controller = "Site", Action = "List", Policy = Permission.Sites, Css = "glyphicon glyphicon-globe"
                    }
                }
            },
            new MenuItem
            {
                InternalId = "System", Name = "System", Css = "glyphicon glyphicon-cog", Items = new MenuItemList
                {
                    new MenuItem
                    {
                        InternalId = "Config", Name = "Config", Controller = "Config", Action = "Edit", Policy = Permission.Config, Css = "glyphicon glyphicon-tasks"
                    }
                }
            }
        };
    }
}