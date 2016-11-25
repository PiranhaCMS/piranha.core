/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;

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
            /// Gets/sets the available items.
            /// </summary>
            public IList<MenuItem> Items { get; set; }
            #endregion

            /// <summary>
            /// Default constructor.
            /// </summary>
            public MenuItem() {
                Items = new List<MenuItem>();
            }
        }
        #endregion

        /// <summary>
        /// The basic manager menu.
        /// </summary>
        public static IList<MenuItem> Items = new List<MenuItem>() {
            new MenuItem() {
                InternalId = "Content", Name = "Content", Css = "glyphicon glyphicon-pencil", Items = new List<MenuItem>() {
                    new MenuItem() {
                        InternalId = "Pages", Name = "Pages", Controller = "Page", Action = "List", Css = "glyphicon glyphicon-duplicate"
                    },
                    new MenuItem() {
                        InternalId = "Posts", Name = "Posts", Controller = "Post", Action = "List", Css = "glyphicon glyphicon-pushpin"
                    },
                    new MenuItem() {
                        InternalId = "Blocks", Name = "Blocks", Controller = "Block", Action = "List", Css = "glyphicon glyphicon-font"
                    },
                    new MenuItem() {
                        InternalId = "Media", Name = "Media", Controller = "Media", Action = "List", Css = "glyphicon glyphicon-picture"
                    },
                    new MenuItem() {
                        InternalId = "Categories", Name = "Categories", Controller = "Category", Action = "List", Css = "glyphicon glyphicon-tags"
                    }
                }
            },
            new MenuItem() {
                InternalId = "Settings", Name = "Settings", Css = "glyphicon glyphicon-wrench", Items = new List<MenuItem>() /* {
                    new MenuItem() {
                        InternalId = "BlockTypes", Name = "Block types", Controller = "BlockType", Action = "List"
                    },
                    new MenuItem() {
                        InternalId = "PageTypes", Name = "Page types", Controller = "PageType", Action = "List"
                    }
                }*/
            },
            new MenuItem() {
                InternalId = "System", Name = "System", Css = "glyphicon glyphicon-cog", Items = new List<MenuItem>() {
                    new MenuItem() {
                        InternalId = "Config", Name = "Config", Controller = "ConfigMgr", Action = "List", Css = "glyphicon glyphicon-wrench"
                    }
                }
            }
        };
    }
}