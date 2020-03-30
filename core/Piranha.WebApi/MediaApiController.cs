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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Models;

namespace Piranha.WebApi
{
    [ApiController]
    [Route("api/media")]
    [Authorize(Policy = Permissions.Media)]
    public class MediaApiController : Controller
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public MediaApiController(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the media asset with the specified id.
        /// </summary>
        /// <param name="id">The media id</param>
        /// <returns>The media asset</returns>
        [HttpGet]
        [Route("{id:Guid}")]
        public Task<Media> GetById(Guid id)
        {
            return _api.Media.GetByIdAsync(id);
        }

        /// <summary>
        /// Gets all of the media assets located in the folder
        /// with the specified id. Not providing a folder id will
        /// return all of the media assets at root level.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("list/{folderId:Guid?}")]
        public Task<IEnumerable<Media>> GetByFolderId(Guid? folderId = null)
        {
            return _api.Media.GetAllByFolderIdAsync(folderId);
        }

        /// <summary>
        /// Gets the media folder structure.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("structure")]
        public Task<MediaStructure> GetStructure()
        {
            return _api.Media.GetStructureAsync();
        }
    }
}