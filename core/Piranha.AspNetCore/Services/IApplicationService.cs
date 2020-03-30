/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Piranha.Models;

namespace Piranha.AspNetCore.Services
{
    public interface IApplicationService
    {
        /// <summary>
        /// Gets the current api.
        /// </summary>
        IApi Api { get; }

        /// <summary>
        /// Gets the site helper.
        /// </summary>
        ISiteHelper Site { get; }

        /// <summary>
        /// Gets the media helper.
        /// </summary>
        IMediaHelper Media { get; }

        /// <summary>
        /// Gets/sets the currently requested URL.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Gets/sets the requested hostname
        /// </summary>
        string Hostname { get; set; }

        /// <summary>
        /// Gets/sets the id of the currently requested page.
        /// </summary>
        Guid PageId { get; set; }

        /// <summary>
        /// Gets/sets the current page.
        /// </summary>
        PageBase CurrentPage { get; set; }

        /// <summary>
        /// Gets/sets the current post.
        /// </summary>
        PostBase CurrentPost { get; set; }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        Task InitAsync(HttpContext context);

        /// <summary>
        /// Gets the gravatar URL from the given parameters.
        /// </summary>
        /// <param name="email">The email address</param>
        /// <param name="size">The requested size</param>
        /// <returns>The gravatar URL</returns>
        string GetGravatarUrl(string email, int size = 0);
    }
}
