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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Services;
using Piranha.Models;

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
        private readonly MediaService _service;
        private readonly IApi _api;

        public MediaApiController(MediaService service, IApi api)
        {
            _service = service;
            _api = api;
        }

        /// <summary>
        /// Gets single media
        /// </summary>
        /// <returns>The list model</returns>
        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var media = await _service.GetById(id);
            if (media == null)
            {
                return NotFound();
            }

            return Ok(media);
        }

        /// <summary>
        /// Gets the image url for the specified dimensions.
        /// </summary>
        /// <param name="id">The unqie id</param>
        /// <param name="width">The optional width</param>
        /// <param name="height">The optional height</param>
        /// <returns>The public url</returns>
        [Route("url/{id}/{width?}/{height?}")]
        [HttpGet]
        public async Task<IActionResult> GetUrl(Guid id, int? width = null, int? height = null)
        {
            if (!width.HasValue)
            {
                var media = await _api.Media.GetByIdAsync(id);

                if (media != null)
                {
                    return Redirect(media.PublicUrl);
                }
                return NotFound();
            }
            else
            {
                return Redirect(await _api.Media.EnsureVersionAsync(id, width.Value, height));
            }
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list/{folderId:Guid?}")]
        [HttpGet]
        public async Task<MediaListModel> List(Guid? folderId = null, MediaType? filter = null)
        {
            return await _service.GetList(folderId, filter);
        }

        [Route("folder/save")]
        [HttpPost]
        public async Task<IActionResult> SaveFolder(MediaFolderModel model)
        {
            try
            {
                await _service.SaveFolder(model);

                var result = await _service.GetList(model.ParentId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = $"The folder <code>{ model.Name }</code> was saved"
                };

                return Ok(result);
            }
            catch (ValidationException e)
            {
                var result = new AliasListModel();
                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
                return BadRequest(result);
            }
        }

        [Route("folder/delete/{id:Guid}")]
        public async Task<IActionResult> DeleteFolder(Guid id)
        {
            try
            {
                var folderId = await _service.DeleteFolder(id);

                var result = await _service.GetList(folderId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = $"The folder was successfully deleted"
                };

                return Ok(result);
            }
            catch (ValidationException e)
            {
                var result = new MediaListModel();
                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
                return BadRequest(result);
            }
        }

        /// <summary>
        /// Adds a new media upload.
        /// </summary>
        /// <param name="model">The upload model</param>
        [HttpPost]
        [Route("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] MediaUploadModel model)
        {
            // Allow for dropzone uploads
            if (!model.Uploads.Any())
            {
                model.Uploads = HttpContext.Request.Form.Files;
            }

            try
            {
                var uploaded = await _service.SaveMedia(model);

                if (uploaded == model.Uploads.Count())
                {
                    return Ok(new StatusMessage
                    {
                        Type = StatusMessage.Success,
                        Body = $"Uploaded all media assets"
                    });
                }
                else if (uploaded == 0)
                {
                    return Ok(new StatusMessage
                    {
                        Type = StatusMessage.Error,
                        Body = $"Could not upload the media assets."
                    });
                }
                else
                {
                    return Ok(new StatusMessage
                    {
                        Type = StatusMessage.Information,
                        Body = $"Uploaded {uploaded} of {model.Uploads.Count()} media assets."
                    });
                }
            }
            catch (Exception e)
            {
                return BadRequest(new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                });
            }
        }

        [Route("delete/{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var folderId = await _service.DeleteMedia(id);
                var result = await _service.GetList(folderId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = $"The media file was successfully deleted"
                };

                return Ok(result);
            }
            catch (ValidationException e)
            {
                var result = new MediaListModel();
                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
                return BadRequest(result);
            }
        }
    }
}