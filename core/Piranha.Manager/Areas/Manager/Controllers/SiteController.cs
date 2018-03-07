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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Areas.Manager.Models;
using Piranha.Manager;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class SiteController : ManagerAreaControllerBase
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SiteController(IApi api) : base(api) { }
        
        /// <summary>
        /// Gets the list view for the current sites.
        /// </summary>
        [Route("manager/sites")]
        [Authorize(Policy = Permission.Sites)]
        public IActionResult List() {
            return View(SiteListModel.Get(api));
        }

        [Route("manager/site/add")]
        [Authorize(Policy = Permission.SitesAdd)]
        public IActionResult Add() {
            return View("Edit", new SiteEditModel());
        }

        [Route("manager/site/{id:Guid}")]
        [Authorize(Policy = Permission.SitesEdit)]
        public IActionResult Edit(Guid id) {
            return View(SiteEditModel.GetById(api, id));
        }

        [Route("manager/site/save")]
        [Authorize(Policy = Permission.SitesSave)]
        public IActionResult Save(SiteEditModel model) {
            try {
                if (model.Save(api)) {
                    SuccessMessage("The site has been saved.");
                    return RedirectToAction("Edit", new { id = model.Site.Id });
                }
                ErrorMessage("The site could not be saved.", false);
                return View("Edit", model);
            } catch (ArgumentException) {
                ErrorMessage("The site could not be saved. Title is mandatory", false);
                return View("Edit", model);                
            }
        }

        [Route("manager/site/delete/{id:Guid}")]
        [Authorize(Policy = Permission.SitesDelete)]
        public IActionResult Delete(Guid id) {
            var site = api.Sites.GetById(id);

            if (site != null) {
                if (!site.IsDefault) {
                    api.Sites.Delete(id);
                    SuccessMessage("The site has been deleted.");
                } else {
                    ErrorMessage("Can't delete the default site.");
                }
            } else {
                ErrorMessage("This site could not be found.");
            }
            return RedirectToAction("List");
        }
    }
}
