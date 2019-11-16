/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;

namespace Piranha.Manager.Extend
{
    public class ModalAction : IAction
    {
        /// <summary>
        /// The private optional script definition.
        /// </summary>
        private ManagerScriptDefinition _script;

        /// <summary>
        /// Gets/sets the unique client id.
        /// </summary>
        public string Uid { get; private set; } = "uid-" + Math.Abs(Guid.NewGuid().GetHashCode()).ToString();

        /// <summary>
        /// Gets/sets the internal id of the action.
        /// </summary>
        public string InternalId { get; set; }

        /// <summary>
        /// Gets/sets the display title for the modal tab.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets/sets the icon css class for the modal tab.
        /// </summary>
        public string Css { get; set; }

        /// <summary>
        /// Gets/sets the name of the global Vue component.
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// Gets/sets the optional script for the global Vue component.
        /// </summary>
        public string ComponentScript {
            get
            {
                return _script?.Src;
            }
            set
            {
                if (_script != null)
                {
                    App.Modules.Manager().Scripts.Remove(_script);
                }

                if (!string.IsNullOrWhiteSpace(value))
                {
                    _script = new ManagerScriptDefinition(value)
                    {
                        RenderBeforeCoreScripts = true
                    };
                    App.Modules.Manager().Scripts.Add(_script);
                }
                else
                {
                    _script = null;
                }
            }
        }
    }
}