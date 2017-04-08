/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Mvc;
using System;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PageController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageController(Api api) : base(api) { }

        /// <summary>
        /// Gets the list view for the pages.
        /// </summary>
        [Route("manager/pages")]
        public ViewResult List() {
            return View(Models.PageListModel.Get(api));
        }

        /// <summary>
        /// Gets the edit view for a page.
        /// </summary>
        /// <param name="id">The page id</param>
        [Route("manager/page/{id}")]
        public IActionResult Edit(string id) {
            return View(Models.PageEditModel.GetById(api, id));
        }

        /// <summary>
        /// Adds a new page of the given type.
        /// </summary>
        /// <param name="type">The page type id</param>
        [Route("manager/page/add/{type}")]
        public IActionResult Add(string type) {
            var sitemap = api.Sites.GetSitemap(onlyPublished: false);
            var model = Models.PageEditModel.Create(api, type);
            model.SortOrder = sitemap.Count;

            return View("Edit", model);
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        //[ValidateAntiForgeryToken] Seems buggy ATM
        [Route("manager/page/save")]
        public IActionResult Save(Models.PageEditModel model) {
            if (model.Save(api)) {
                SuccessMessage("The page has been saved.");
                return RedirectToAction("Edit", new { id = model.Id });
            } else {
                ErrorMessage("The page could not be saved.", false);
                return View("Edit", model);
            }
        }

        /// <summary>
        /// Saves and publishes the given page model.
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        //[ValidateAntiForgeryToken] Seems buggy ATM
        [Route("manager/page/publish")]
        public IActionResult Publish(Models.PageEditModel model) {
            if (model.Save(api, true)) {
                SuccessMessage("The page has been published.");
                return RedirectToAction("Edit", new { id = model.Id });
            } else {
                ErrorMessage("The page could not be published.", false);
                return View(model);
            }
        }

        /// <summary>
        /// Saves and unpublishes the given page model.
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        //[ValidateAntiForgeryToken] Seems buggy ATM
        [Route("manager/page/unpublish")]
        public IActionResult UnPublish(Models.PageEditModel model) {
            if (model.Save(api, false)) {
                SuccessMessage("The page has been unpublished.");
                return RedirectToAction("Edit", new { id = model.Id });
            } else {
                ErrorMessage("The page could not be unpublished.", false);
                return View(model);
            }
        }

        /// <summary>
        /// Moves a page to match the given structure.
        /// </summary>
        /// <param name="structure">The page structure</param>
        [HttpPost]
        [Route("manager/pages/move")]
        public IActionResult Move([FromBody]Models.PageStructureModel structure) {
            for (var n = 0; n < structure.Items.Count; n++) {
                var moved = MovePage(structure.Items[n], n);
                if (moved)
                    break;
            }
            return View("Partial/_Sitemap", api.Sites.GetSitemap(onlyPublished: false));
        }

        /// <summary>
        /// Deletes the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("manager/page/delete/{id}")]
        public IActionResult Delete(string id) {
            api.Pages.Delete(id);
            SuccessMessage("The page has been deleted");
            return RedirectToAction("List");
        }

        #region Private methods
        private bool MovePage(Models.PageStructureModel.PageStructureItem page, int sortOrder = 1, string parentId = null) {
            var model = api.Pages.GetById(page.Id);

            if (model != null) {
                if (model.ParentId != parentId || model.SortOrder != sortOrder) {
                    // Move the page in the structure.
                    api.Pages.Move(model, parentId, sortOrder);

                    // We only move one page at a time so we're done
                    return true;
                }

                for (var n = 0; n < page.Children.Count; n++) {
                    var moved = MovePage(page.Children[n], n, page.Id);

                    if (moved)
                        return true;
                }
            }
            return false;
        }
        #endregion
    }
}
