/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/piranhacms/piranha
 * 
 */

using System;

namespace Piranha.Data
{
    public sealed class Alias : IModel
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets/sets the id of the site this alias is for.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the alias url.
        /// </summary>
        public string AliasUrl { get; set; }

        /// <summary>
        /// Gets/sets the url of the redirect.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets if this is a permanent or temporary
        /// redirect.
        /// </summary>
        public Models.RedirectType Type { get; set; } = Models.RedirectType.Temporary;

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }

        /// <summary>
        /// Gets/sets the site this alias is for.
        /// </summary>
        /// <returns></returns>
        public Site Site { get; set; }
    }
}