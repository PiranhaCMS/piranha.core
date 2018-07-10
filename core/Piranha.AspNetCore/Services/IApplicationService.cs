/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Http;
using System;
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
        /// Gets the currently requested URL.
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// Gets the id of the currently requested page.
        /// </summary>
        Guid PageId { get; set; }

        /// <summary>
        /// Initializes the service.
        /// </summary>
        void Init(HttpContext context);
    }
}
