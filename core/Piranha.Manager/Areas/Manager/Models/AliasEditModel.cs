/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Piranha.Areas.Manager.Models
{
    public class AliasEditModel
    {
        /// <summary>
        /// Gets/sets the site id.
        /// </summary>
        public Guid SiteId { get; set; }

        /// <summary>
        /// Gets/sets the alias url.
        /// </summary>
        public string AliasUrl { get; set; }

        /// <summary>
        /// Gets/sets the redirect url.
        /// </summary>
        public string RedirectUrl { get; set; }

        /// <summary>
        /// Gets/sets if the redirect is permanent.
        /// </summary>
        public bool IsPermanent { get; set; }
    }
}