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
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class MediaApiController : Controller
    {
        private readonly MediaService _service;
        private readonly IApi _api;
        private readonly ManagerLocalizer _localizer;

        public MediaApiController(MediaService service, IApi api, ManagerLocalizer localizer)
        {
            _service = service;
            _api = api;
            _localizer = localizer;
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
        public async Task<MediaListModel> List(Guid? folderId = null, [FromQuery]MediaType? filter = null, [FromQuery] int? width = null, [FromQuery] int? height = null)
        {
            return await _service.GetList(folderId, filter, width, height);
        }

        /// <summary>
        /// Saves the meta information for the given media asset.
        /// </summary>
        /// <param name="model">The media model</param>
        [Route("meta/save")]
        [HttpPost]
        public async Task<IActionResult> SaveMeta(MediaListModel.MediaItem model)
        {
            if (await _service.SaveMeta(model))
            {
                return Ok(new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = _localizer.Media["The meta information was succesfully updated"]
                });
            }
            else
            {
                return Ok(new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = _localizer.Media["An error occured when updating the meta information"]
                });
            }
        }

        [Route("folder/save")]
        [HttpPost]
        [Authorize(Policy = Permission.MediaAddFolder)]
        public async Task<IActionResult> SaveFolder(MediaFolderModel model, MediaType? filter = null)
        {
            try
            {
                await _service.SaveFolder(model);

                var result = await _service.GetList(model.ParentId, filter);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = String.Format(_localizer.Media["The folder <code>{0}</code> was saved"], model.Name)
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
        [HttpGet]
        [Authorize(Policy = Permission.MediaDeleteFolder)]
        public async Task<IActionResult> DeleteFolder(Guid id)
        {
            try
            {
                var folderId = await _service.DeleteFolder(id);

                var result = await _service.GetList(folderId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = _localizer.Media["The folder was successfully deleted"]
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
        [Route("upload")]
        [HttpPost]
        [Consumes("multipart/form-data")]
        [Authorize(Policy = Permission.MediaAdd)]
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
                        Body = _localizer.Media["Uploaded all media assets"]
                    });
                }
                else if (uploaded == 0)
                {
                    return Ok(new StatusMessage
                    {
                        Type = StatusMessage.Error,
                        Body = _localizer.Media["Could not upload the media assets"]
                    });
                }
                else
                {
                    return Ok(new StatusMessage
                    {
                        Type = StatusMessage.Information,
                        Body = String.Format(_localizer.Media["Uploaded {0} of {1} media assets"], uploaded, model.Uploads.Count())
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

        [Route("move/{mediaId}/{folderId?}")]
        [HttpGet]
        [Authorize(Policy = Permission.MediaEdit)]
        public async Task<IActionResult> Move(Guid mediaId, Guid? folderId)
        {
            try
            {
                if (mediaId != folderId)
                {
                    var media = await _api.Media.GetByIdAsync(mediaId);
                    if (media != null)
                    {
                        await _api.Media.MoveAsync(media, folderId);

                        return Ok(new StatusMessage
                        {
                            Type = StatusMessage.Success,
                            Body = $"{media.Filename} was successfully moved."
                        });
                    }

                    var folder = await _api.Media.GetFolderByIdAsync(mediaId);
                    if (folder != null)
                    {
                        folder.ParentId = folderId;
                        await _api.Media.SaveFolderAsync(folder);

                        return Ok(new StatusMessage
                        {
                            Type = StatusMessage.Success,
                            Body = $"{folder.Name} was successfully moved."
                        });
                    }
                    return BadRequest(new StatusMessage
                    {
                        Type = StatusMessage.Error,
                        Body = _localizer.Media["The media file was not found."]
                    });
                }
                return BadRequest();
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
        [HttpGet]
        [Authorize(Policy = Permission.MediaDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var folderId = await _service.DeleteMedia(id);
                var result = await _service.GetList(folderId);

                result.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = _localizer.Media["The media file was successfully deleted"]
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