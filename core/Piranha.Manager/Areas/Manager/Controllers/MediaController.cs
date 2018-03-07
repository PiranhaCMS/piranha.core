/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Areas.Manager.Models;
using Piranha.Data;
using Piranha.Manager;
using Piranha.Models;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class MediaController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public MediaController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the list view for the media.
        /// </summary>
        /// <param name="folderId">The optional folder id</param>
        [Route("manager/media/{folderId:Guid?}")]
        [Authorize(Policy = Permission.Media)]
        public IActionResult List(Guid? folderId = null)
        {
            return View("List", MediaListModel.Get(Api, folderId));
        }

        /// <summary>
        /// Adds a new media upload.
        /// </summary>
        /// <param name="model">The upload model</param>
        [HttpPost]
        [Route("manager/media/add")]
        [Authorize(Policy = Permission.MediaAdd)]
        public async Task<IActionResult> Add(MediaUploadModel model)
        {
            var uploaded = 0;

            foreach (var upload in model.Uploads)
            {
                if (upload.Length <= 0 || string.IsNullOrWhiteSpace(upload.ContentType))
                {
                    continue;
                }

                using (var stream = upload.OpenReadStream())
                {
                    await Api.Media.SaveAsync(new StreamMediaContent
                    {
                        Id = model.Uploads.Count() == 1 ? model.Id : null,
                        FolderId = model.ParentId,
                        Filename = Path.GetFileName(upload.FileName),
                        Data = stream
                    });
                    uploaded++;
                }
            }
            if (uploaded == model.Uploads.Count())
            {
                SuccessMessage("Uploaded all media assets.");
            }
            else if (uploaded == 0)
            {
                ErrorMessage("Could not upload the media assets.");
            }
            else
            {
                InformationMessage($"Uploaded {uploaded} of {model.Uploads.Count()} media assets.");
            }

            return RedirectToAction("List", new { folderId = model.ParentId });
        }

        /// <summary>
        /// Adds a new media upload.
        /// </summary>
        /// <param name="model">The upload model</param>
        [HttpPost]
        [Route("manager/media/modal/add")]
        [Authorize(Policy = Permission.MediaAdd)]
        public async Task<IActionResult> ModalAdd(MediaUploadModel model)
        {
            var uploaded = 0;

            foreach (var upload in model.Uploads)
            {
                if (upload.Length <= 0 || string.IsNullOrWhiteSpace(upload.ContentType))
                {
                    continue;
                }

                using (var stream = upload.OpenReadStream())
                {
                    await Api.Media.SaveAsync(new StreamMediaContent
                    {
                        Id = model.Uploads.Count() == 1 ? model.Id : null,
                        FolderId = model.ParentId,
                        Filename = Path.GetFileName(upload.FileName),
                        Data = stream
                    });
                    uploaded++;
                }
            }
            return Modal(model.ParentId);
        }

        /// <summary>
        /// Adds a new media folder
        /// </summary>
        /// <param name="model">The model</param>
        [HttpPost]
        [Route("manager/media/addfolder")]
        [Authorize(Policy = Permission.MediaAddFolder)]
        public IActionResult AddFolder(MediaFolderModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.Name))
            {
                Api.Media.SaveFolder(new MediaFolder
                {
                    ParentId = model.ParentId,
                    Name = model.Name
                });
                SuccessMessage($"Added folder \"{model.Name}\".");
            }
            else
            {
                ErrorMessage("Name is mandatory when creating a new folder.");
            }
            return RedirectToAction("List", new { folderId = model.ParentId });
        }

        /// <summary>
        /// Deletes the media upload with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("/manager/media/delete/{id:Guid}")]
        [Authorize(Policy = Permission.MediaDelete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var media = Api.Media.GetById(id);

            if (media != null)
            {
                await Api.Media.DeleteAsync(media);
                SuccessMessage($"Deleted \"{media.Filename}\".");
                return RedirectToAction("List", new { folderId = media.FolderId });
            }
            ErrorMessage("Could not delete the uploaded media.");
            return RedirectToAction("List", new { folderId = "" });
        }

        /// <summary>
        /// Deletes the folder with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("/manager/media/delete/folder/{id:Guid}")]
        [Authorize(Policy = Permission.MediaDeleteFolder)]
        public IActionResult DeleteFolder(Guid id)
        {
            var folder = Api.Media.GetFolderById(id);

            if (folder != null)
            {
                var media = Api.Media.GetAll(folder.Id);

                if (!media.Any())
                {
                    Api.Media.DeleteFolder(folder);
                    SuccessMessage($"Deleted folder \"{folder.Name}\".");
                    return RedirectToAction("List", new { folderId = folder.ParentId });
                }
                ErrorMessage($"The folder \"{folder.Name}\" is not empty.");
                return RedirectToAction("List", new { folderId = folder.ParentId });
            }
            ErrorMessage("Could not delete the folder.");
            return RedirectToAction("List", new { folderId = "" });
        }

        [Route("/manager/media/modal/{folderId?}")]
        public IActionResult Modal(Guid? folderId = null, string filter = null)
        {
            MediaType? type = null;

            switch (filter)
            {
                case "image":
                    type = MediaType.Image;
                    break;
                case "document":
                    type = MediaType.Document;
                    break;
                case "video":
                    type = MediaType.Video;
                    break;
            }

            return View("Modal", MediaListModel.Get(Api, folderId, type));
        }
    }
}