/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

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
        [Route("manager/pages/{pageId?}")]
        [Authorize(Policy = Permission.Pages)]
        public ViewResult List(string pageId = null) {
            return ListSite(null, pageId);
        }

        /// <summary>
        /// Gets the list view for the pages of the specified site.
        /// </summary>
        [Route("manager/pages/site/{siteId}/{pageId?}")]
        [Authorize(Policy = Permission.Pages)]
        public ViewResult ListSite(string siteId, string pageId = null) {
            var model = Models.PageListModel.Get(api, siteId, pageId);
            var defaultSite = api.Sites.GetDefault();

            // TODO!
            //
            // This doesn't really work for multiple users but is
            // rather a proof of context. Menus needs to be changed
            // per user session rather than globally.
            Piranha.Manager.Menu
                .Items["Content"]
                .Items["Pages"]
                .Action = string.IsNullOrEmpty(siteId) ? "List" : "ListSite";
            Piranha.Manager.Menu
                .Items["Content"]
                .Items["Pages"]
                .Params = new {
                    pageId = "",
                    siteId = model.SiteId != defaultSite.Id ? model.SiteId : ""
                };
            return View("List", model);
        }

        /// <summary>
        /// Gets the edit view for a page.
        /// </summary>
        /// <param name="id">The page id</param>
        [Route("manager/page/{id}")]
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult Edit(string id) {
            return View(Models.PageEditModel.GetById(api, id));
        }

        /// <summary>
        /// Adds a new page of the given type.
        /// </summary>
        /// <param name="type">The page type id</param>
        /// <param name="siteId">The optional site id</param>
        [Route("manager/page/add/{type}/{siteId?}")]
        [Authorize(Policy = Permission.PagesAdd)]
        public IActionResult Add(string type, string siteId = null) {
            var sitemap = api.Sites.GetSitemap(onlyPublished: false);
            var model = Models.PageEditModel.Create(api, type, siteId);
            model.SortOrder = sitemap.Count;

            return View("Edit", model);
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        [Route("manager/page/save")]
        [Authorize(Policy = Permission.PagesSave)]
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
        [Route("manager/page/publish")]
        [Authorize(Policy = Permission.PagesPublish)]
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
        [Route("manager/page/unpublish")]
        [Authorize(Policy = Permission.PagesPublish)]
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
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult Move([FromBody]Models.PageStructureModel structure) {
            for (var n = 0; n < structure.Items.Count; n++) {
                var moved = MovePage(structure.Items[n], n);
                if (moved)
                    break;
            }
            using (var config = new Config(api)) {
                return View("Partial/_Sitemap", new Models.SitemapModel() {
                    Sitemap = api.Sites.GetSitemap(onlyPublished: false),
                    ExpandedLevels = config.ManagerExpandedSitemapLevels
                });
            }
        }

        /// <summary>
        /// Deletes the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("manager/page/delete/{id}")]
        [Authorize(Policy = Permission.PagesDelete)]
        public IActionResult Delete(string id) {
            api.Pages.Delete(id);
            SuccessMessage("The page has been deleted");
            return RedirectToAction("List");
        }

        /// <summary>
        /// Adds a new region to a page.
        /// </summary>
        /// <param name="model">The model</param>
        [HttpPost]
        [Route("manager/page/region")]
        [Authorize(Policy = Permission.Pages)]
        public IActionResult AddRegion([FromBody]Models.PageRegionModel model) {
            var pageType = api.PageTypes.GetById(model.PageTypeId);

            if (pageType != null) {
                var regionType = pageType.Regions.SingleOrDefault(r => r.Id == model.RegionTypeId);

                if (regionType != null) {
                    var region = Piranha.Models.DynamicPage.CreateRegion(api,
                        model.PageTypeId, model.RegionTypeId);

                    var editModel = (Models.PageEditRegionCollection)Models.PageEditModel.CreateRegion(regionType, 
                        new List<object>() { region});

                    ViewData.TemplateInfo.HtmlFieldPrefix = $"Regions[{model.RegionIndex}].FieldSets[{model.ItemIndex}]";
                    return View("EditorTemplates/PageEditRegionItem", editModel.FieldSets[0]);
                }
            }
            return new NotFoundResult();
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
