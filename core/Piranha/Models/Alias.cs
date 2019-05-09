/*
 * Copyright (c) 2018-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace Piranha.Models
{
    [Serializable]
    public sealed class Alias : Alias<Guid> { }

    [Serializable]
    public abstract class Alias<TKey>
    {
        /// <summary>
        /// Gets/sets the unique id.
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// Gets/sets the id of the site this alias is for.
        /// </summary>
        public TKey SiteId { get; set; }

        /// <summary>
        /// Gets/sets the alias url.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string AliasUrl { get; set; }

        /// <summary>
        /// Gets/sets the url of the redirect.
        /// </summary>
        [Required]
        [StringLength(256)]
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets if this is a permanent or temporary
        /// redirect.
        /// </summary>
        public RedirectType Type { get; set; } = RedirectType.Temporary;

        /// <summary>
        /// Gets/sets the created date.
        /// </summary>
        public DateTime Created { get; set; }

        /// <summary>
        /// Gets/sets the last modification date.
        /// </summary>
        public DateTime LastModified { get; set; }
    }
}