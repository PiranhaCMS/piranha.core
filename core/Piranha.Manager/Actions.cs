/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Manager.Extend;

namespace Piranha.Manager
{
    /// <summary>
    /// Static class for defining the manager menu.
    /// </summary>
    public static class Actions
    {
        public sealed class ModalActions
        {
            /// <summary>
            /// Gets the available actions for the media preview modal.
            /// </summary>
            public ActionList<ModalAction> MediaPreview { get; private set; } = new ActionList<ModalAction>();

            /// <summary>
            /// Default constructor.
            /// </summary>
            internal ModalActions() { }
        }

        public sealed class ToolbarActions
        {
            /// <summary>
            /// Gets the available actions for the alias view.
            /// </summary>
            public ActionList<ToolbarAction> AliasEdit { get; private set; } = new ActionList<ToolbarAction>
            {
                new ToolbarAction
                {
                    InternalId = "Add",
                    ActionView = "Partial/Actions/_AliasAdd"
                }
            };

            /// <summary>
            /// Gets the available actions for the comments view.
            /// </summary>
            public ActionList<ToolbarAction> CommentList { get; private set; } = new ActionList<ToolbarAction>();

            /// <summary>
            /// Gets the available actions for the config view.
            /// </summary>
            public ActionList<ToolbarAction> ConfigEdit { get; private set; } = new ActionList<ToolbarAction>
            {
                new ToolbarAction
                {
                    InternalId = "Save",
                    ActionView = "Partial/Actions/_ConfigSave"
                }
            };

            /// <summary>
            /// Gets the available actions for media list view.
            /// </summary>
            public ActionList<ToolbarAction> MediaList { get; private set; } = new ActionList<ToolbarAction>();

            /// <summary>
            /// Gets the available actions for the module list view.
            /// </summary>
            /// <returns></returns>
            public ActionList<ToolbarAction> ModuleList { get; private set; } = new ActionList<ToolbarAction>();

            /// <summary>
            /// Gets the available actions for the page edit view.
            /// </summary>
            public ActionList<ToolbarAction> PageEdit { get; private set; } = new ActionList<ToolbarAction>
            {
                new ToolbarAction
                {
                    InternalId = "Settings",
                    ActionView = "Partial/Actions/_PageSettings"
                },
                new ToolbarAction
                {
                    InternalId = "Preview",
                    ActionView = "Partial/Actions/_PagePreview"
                },
                new ToolbarAction
                {
                    InternalId = "Save",
                    ActionView = "Partial/Actions/_PageSave"
                },
                new ToolbarAction
                {
                    InternalId = "Delete",
                    ActionView = "Partial/Actions/_PageDelete"
                }
            };

            /// <summary>
            /// Gets the actions available for the page list view.
            /// </summary>
            public ActionList<ToolbarAction> PageList { get; private set; } = new ActionList<ToolbarAction>
            {
                new ToolbarAction
                {
                    InternalId = "AddSite",
                    ActionView = "Partial/Actions/_PageListAddSite"
                },
            };

            /// <summary>
            /// Gets the available actions for the page edit view.
            /// </summary>
            public ActionList<ToolbarAction> PostEdit { get; private set; } = new ActionList<ToolbarAction>
            {
                new ToolbarAction
                {
                    InternalId = "Settings",
                    ActionView = "Partial/Actions/_PostSettings"
                },
                new ToolbarAction
                {
                    InternalId = "Preview",
                    ActionView = "Partial/Actions/_PostPreview"
                },
                new ToolbarAction
                {
                    InternalId = "Save",
                    ActionView = "Partial/Actions/_PostSave"
                },
                new ToolbarAction
                {
                    InternalId = "Delete",
                    ActionView = "Partial/Actions/_PostDelete"
                }
            };

            /// <summary>
            /// Default constructor.
            /// </summary>
            internal ToolbarActions() { }
        }

        /// <summary>
        /// Gets the available modal actions.
        /// </summary>
        public static ModalActions Modals { get; private set; } = new ModalActions();

        /// <summary>
        /// Gets/sets the available toolbar actions.
        /// </summary>
        public static ToolbarActions Toolbars { get; private set; } = new ToolbarActions();
    }
}