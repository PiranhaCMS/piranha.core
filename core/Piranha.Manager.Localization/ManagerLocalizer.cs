/*
 * Copyright (c) .NET Foundation and Contributors
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
        /// Gets/sets comment string resource.
        /// </summary>
        /// <value></value>
        public IStringLocalizer<Localization.Comment> Comment { get; private set; }

        /// <summary>
        /// Gets/sets config string resources.
        /// </summary>
        public IStringLocalizer<Localization.Config> Config { get; private set; }

        /// <summary>
        /// Gets/sets general string resources.
        /// </summary>
        public IStringLocalizer<Localization.General> General { get; private set; }

        /// <summary>
        /// Gets/sets security string resources.
        /// </summary>
        /// <value></value>
        public IStringLocalizer<Localization.Security> Security { get; private set; }

        /// <summary>
        /// Gets/sets media string localization.
        /// </summary>
        public IStringLocalizer<Localization.Media> Media { get; private set; }

        /// <summary>
        /// Gets/sets menu string localization.
        /// </summary>
        public IStringLocalizer<Localization.Menu> Menu { get; private set; }

        /// <summary>
        /// Gets/sets module string localization.
        /// </summary>
        public IStringLocalizer<Localization.Module> Module { get; private set; }

        /// <summary>
        /// Gets/sets page string localization.
        /// </summary>
        public IStringLocalizer<Localization.Page> Page { get; private set; }

        /// <summary>
        /// Gets/sets post string localization.
        /// </summary>
        public IStringLocalizer<Localization.Post> Post { get; private set; }

        /// <summary>
        /// Gets/sets site string localization.
        /// </summary>
        public IStringLocalizer<Localization.Site> Site { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public ManagerLocalizer(
            IStringLocalizer<Localization.Alias> alias,
            IStringLocalizer<Localization.Comment> comment,
            IStringLocalizer<Localization.Config> config,
            IStringLocalizer<Localization.General> general,
            IStringLocalizer<Localization.Security> security,
            IStringLocalizer<Localization.Media> media,
            IStringLocalizer<Localization.Menu> menu,
            IStringLocalizer<Localization.Module> module,
            IStringLocalizer<Localization.Page> page,
            IStringLocalizer<Localization.Post> post,
            IStringLocalizer<Localization.Site> site)
        {
            Alias = alias;
            Comment = comment;
            Config = config;
            General = general;
            Security = security;
            Module = module;
            Media = media;
            Menu = menu;
            Page = page;
            Post = post;
            Site = site;
        }
    }
}
