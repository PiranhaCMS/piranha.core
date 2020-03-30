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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Models;

namespace Piranha.WebApi
{
    [ApiController]
    [Route("api/sitemap")]
    [Authorize(Policy = Permissions.Sitemap)]
    public class SitemapApiController : Controller
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SitemapApiController(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the public sitemap for the default or specified site.
        /// </summary>
        /// <param name="id">The optional site id</param>
        /// <returns>The sitemap</returns>
        [HttpGet]
        [Route("{id:Guid?}")]
        public Task<Sitemap> GetById(Guid? id = null)
        {
            return _api.Sites.GetSitemapAsync(id);
        }
    }
}