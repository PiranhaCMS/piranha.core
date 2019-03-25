/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using Piranha.Services;


namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for alias management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/media")]
    [ApiController]
    public class MediaApiController : Controller
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MediaApiController(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets single media
        /// </summary>
        /// <returns>The list model</returns>
        [Route("{id}")]
        [HttpGet]
        public object Get(Guid id)
        {
            var media = _api.Media.GetById(id);
            if (media != null)
                return media;

            media = new Piranha.Models.Media
            {
                Id = Guid.NewGuid(),
                Filename = "DCS9783_BBQ.jpg",
                ContentType = "image/jpeg",
                Type = Piranha.Models.MediaType.Image,
                Size = 54790,
                Height = 470,
                Width = 1600,
                Created = DateTime.Now,
                LastModified = DateTime.Now,
                PublicUrl = "https://picsum.photos/1000/700"
            };

            return media;

            //return NotFound();
        }

        /// <summary>
        /// Gets a list of media.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list/{folderId?}")]
        [HttpGet]
        public IEnumerable<Piranha.Models.Media> List(Guid? folderId)
        {
            return _api.Media.GetAll(folderId);
        }
    }
}