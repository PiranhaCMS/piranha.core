/*
 * Copyright (c) 2017-2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Areas.Manager.Models;
using Piranha.Manager;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PostController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PostController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the edit view for a post.
        /// </summary>
        /// <param name="id">The post id</param>
        [Route("manager/post/{id:Guid}")]
        [Authorize(Policy = Permission.PostsEdit)]
        public IActionResult Edit(Guid id) {
            return View(PostEditModel.GetById(api, id));
        }

        /// <summary>
        /// Adds a new page of the given type.
        /// </summary>
        /// <param name="type">The page type id</param>
        /// <param name="blogId">The blog id</param>
        [Route("manager/post/add/{type}/{blogId:Guid}")]
        [Authorize(Policy = Permission.PostsEdit)]
        public IActionResult Add(string type, Guid blogId) {
            var model = PostEditModel.Create(api, type, blogId);

            return View("Edit", model);
        }        

        /// <summary>
        /// Saves the given post model
        /// </summary>
        /// <param name="model">The post model</param>
        [HttpPost]
        [Route("manager/post/save")]
        [Authorize(Policy = Permission.PostsSave)]
        public IActionResult Save(PostEditModel model) {
            // Validate
            if (string.IsNullOrWhiteSpace(model.Title)) {
                ErrorMessage("The post could not be saved. Title is mandatory", false);
                return View("Edit", model.Refresh(api));
            }
            if (string.IsNullOrWhiteSpace(model.SelectedCategory)) {
                ErrorMessage("The post could not be saved. Category is mandatory", false);
                return View("Edit", model.Refresh(api));
            }

            // Save
            if (model.Save(api)) {
                SuccessMessage("The post has been saved.");
                return RedirectToAction("Edit", new { id = model.Id });
            }
            ErrorMessage("The post could not be saved.", false);
            return View("Edit", model.Refresh(api));
        }

        /// <summary>
        /// Saves and publishes the given post model.
        /// </summary>
        /// <param name="model">The post model</param>
        [HttpPost]
        [Route("manager/post/publish")]
        [Authorize(Policy = Permission.PostsPublish)]
        public IActionResult Publish(PostEditModel model) {
            // Validate
            if (string.IsNullOrWhiteSpace(model.Title)) {
                ErrorMessage("The post could not be saved. Title is mandatory", false);
                return View("Edit", model.Refresh(api));
            }
            if (string.IsNullOrWhiteSpace(model.SelectedCategory)) {
                ErrorMessage("The post could not be saved. Category is mandatory", false);
                return View("Edit", model.Refresh(api));
            }

            // Save
            if (model.Save(api, true)) {
                SuccessMessage("The post has been published.");
                return RedirectToAction("Edit", new { id = model.Id });
            }
            ErrorMessage("The post could not be published.", false);
            return View(model);
        }

        /// <summary>
        /// Saves and unpublishes the given post model.
        /// </summary>
        /// <param name="model">The post model</param>
        [HttpPost]
        [Route("manager/post/unpublish")]
        [Authorize(Policy = Permission.PostsPublish)]
        public IActionResult UnPublish(PostEditModel model) {
            if (model.Save(api, false)) {
                SuccessMessage("The post has been unpublished.");
                return RedirectToAction("Edit", new { id = model.Id });
            }
            ErrorMessage("The post could not be unpublished.", false);
            return View(model);
        }        

        /// <summary>
        /// Deletes the post with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("manager/post/delete/{id:Guid}")]
        [Authorize(Policy = Permission.PostsDelete)]
        public IActionResult Delete(Guid id) {
            var post = api.Posts.GetById(id);

            if (post != null) {
                api.Posts.Delete(post);

                SuccessMessage("The post has been deleted");

                return RedirectToAction("Edit", "Page", new { id = post.BlogId });
            }
            ErrorMessage("The post could not be deleted");
            return RedirectToAction("List", "Page", new { id = "" });
        }

        /// <summary>
        /// Gets the post modal for the specified blog.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="blogId">The blog id</param>
        [Route("manager/post/modal/{siteId:Guid?}/{blogId:Guid?}")]
        [Authorize(Policy = Permission.Posts)]
        public IActionResult Modal(Guid? siteId = null, Guid? blogId = null) {
            return View(PostModalModel.GetByBlogId(api, siteId, blogId));
        }  
    }
}
