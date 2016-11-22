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
    public class CategoryController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constroller.
        /// </summary>
        /// <param name="api">The current api</param>
        public CategoryController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the category list view.
        /// </summary>
        [Route("manager/categories")]
        public IActionResult List() {
            return View(Models.CategoryListModel.Get(api));
        }

        /// <summary>
        /// Gets the edit view for the category with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("manager/category/{id:Guid?}")]
        public IActionResult Edit(Guid? id = null) {
            if (id.HasValue)
                return View(Models.CategoryEditModel.GetById(api, id.Value));
            else return View(new Models.CategoryEditModel());
        }

        /// <summary>
        /// Saves the given model.
        /// </summary>
        /// <param name="model">The category model</param>
        [HttpPost]
        [Route("manager/category/save")]
        public IActionResult Save(Models.CategoryEditModel model) {
            if (ModelState.IsValid) {
                if (model.Save(api))
                    return RedirectToAction("List");
            }
            return View("Edit", model);
        }

        /// <summary>
        /// Deletes the category with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("manager/category/delete/{id:Guid}")]
        public IActionResult Delete(Guid id) {
            api.Categories.Delete(id);
            return RedirectToAction("List");
        }
    }
}
