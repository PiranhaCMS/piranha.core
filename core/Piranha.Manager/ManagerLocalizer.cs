/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.Extensions.Localization;

namespace Piranha.Manager
{
    public class ManagerLocalizer
    {
        /// <summary>
        /// Gets/sets alias string resources.
        /// </summary>
        public IStringLocalizer<Localization.Alias> Alias { get; private set; }

        /// <summary>
        /// Gets/sets general string resources.
        /// </summary>
        public IStringLocalizer<Localization.General> General { get; private set; }

        /// <summary>
        /// Gets/sets media string localization.
        /// </summary>
        public IStringLocalizer<Localization.Media> Media { get; private set; }

        /// <summary>
        /// Gets/sets page string localization.
        /// </summary>
        public IStringLocalizer<Localization.Page> Page { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ManagerLocalizer(
            IStringLocalizer<Localization.Alias> alias,
            IStringLocalizer<Localization.General> general,
            IStringLocalizer<Localization.Media> media,
            IStringLocalizer<Localization.Page> page)
        {
            Alias = alias;
            General = general;
            Media = media;
            Page = page;
        }
    }
}
