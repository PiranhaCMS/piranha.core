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
using Piranha.Models;

namespace Piranha.AspNetCore.Services
{
    public interface ISiteHelper
    {
        /// <summary>
        /// Gets the id of the currently requested site.
        /// </summary>
        Guid Id { get; set; }

        /// <summary>
        /// Gets the sitemap of the currently requested site.
        /// </summary>
        Sitemap Sitemap { get; set; }

        /// <summary>
        /// Gets the site content for the current site.
        /// </summary>
        /// <typeparam name="T">The content type</typeparam>
        /// <returns>The site content model</returns>
        T GetContent<T>() where T : SiteContent<T>;
    }
}