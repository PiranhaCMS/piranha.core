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
    [Route("api/page")]
    [Authorize(Policy = Permissions.Pages)]
    public class PageApiController : Controller
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageApiController(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the full page model for the page with
        /// the specified id.
        /// </summary>
        /// <param name="id">The page id</param>
        /// <returns>The page model</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        public Task<PageBase> GetById(Guid id)
        {
            return _api.Pages.GetByIdAsync<PageBase>(id);
        }

        /// <summary>
        /// Gets the full page model for the page with
        /// the specified slug in the default site.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The page model</returns>
        [HttpGet]
        [Route("{slug}")]
        public Task<PageBase> GetBySlug(string slug)
        {
            return _api.Pages.GetBySlugAsync<PageBase>(slug);
        }

        /// <summary>
        /// Gets the full page model for the page with
        /// the specified slug and site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="slug">The slug</param>
        /// <returns>The page model</returns>
        [HttpGet]
        [Route("{siteId}/{slug}")]
        public Task<PageBase> GetBySlugAndSite(Guid siteId, string slug)
        {
            return _api.Pages.GetBySlugAsync<PageBase>(slug, siteId);
        }

        /// <summary>
        /// Gets the page info model for the page with
        /// the specified id.
        /// </summary>
        /// <param name="id">The page id</param>
        /// <returns>The page model</returns>
        [HttpGet]
        [Route("info/{id:Guid}")]
        public Task<PageInfo> GetInfoById(Guid id)
        {
            return _api.Pages.GetByIdAsync<PageInfo>(id);
        }

        /// <summary>
        /// Gets the page info model for the page with
        /// the specified slug in the default site.
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The page model</returns>
        [HttpGet]
        [Route("info/{slug}")]
        public Task<PageInfo> GetInfoBySlug(string slug)
        {
            return _api.Pages.GetBySlugAsync<PageInfo>(slug);
        }

        /// <summary>
        /// Gets the page info model for the page with
        /// the specified slug and site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="slug">The slug</param>
        /// <returns>The page model</returns>
        [HttpGet]
        [Route("info/{siteId}/{slug}")]
        public Task<PageInfo> GetInfoBySlugAndSite(Guid siteId, string slug)
        {
            return _api.Pages.GetBySlugAsync<PageInfo>(slug, siteId);
        }
    }
}