﻿/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Areas.Manager.Models;
using Piranha.Manager;
using Piranha.Models;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PageController : ManagerAreaControllerBase
    {
        private const string COOKIE_SELECTEDSITE = "PiranhaManager_SelectedSite";

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public PageController(IApi api) : base(api) { }

        /// <summary>
        /// Gets the list view for the pages.
        /// </summary>
        [Route("manager/pages/{pageId?}")]
        [Authorize(Policy = Permission.Pages)]
        public ViewResult List(string pageId = null) {
            // Get the currently selected site from the request cookies
            var siteId = Request.Cookies[COOKIE_SELECTEDSITE];
            Guid? site = null;

            if (!string.IsNullOrWhiteSpace(siteId))
                site = new Guid(siteId);

            return ListSite(site, pageId);
        }

        /// <summary>
        /// Gets the list view for the pages of the specified site.
        /// </summary>
        [Route("manager/pages/site/{siteId:Guid?}/{pageId?}")]
        [Authorize(Policy = Permission.Pages)]
        public ViewResult ListSite(Guid? siteId, string pageId = null) {
            var model = PageListModel.Get(api, siteId, pageId);
            var defaultSite = api.Sites.GetDefault();

            // Store a cookie on our currently selected site
            if (siteId.HasValue)
                Response.Cookies.Append(COOKIE_SELECTEDSITE, siteId.ToString());
            else Response.Cookies.Delete(COOKIE_SELECTEDSITE); 

            return View("List", model);
        }

        /// <summary>
        /// Gets the edit view for a page.
        /// </summary>
        /// <param name="id">The page id</param>
        [Route("manager/page/{id:Guid}")]
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult Edit(Guid id) {
            return View(PageEditModel.GetById(api, id));
        }

        /// <summary>
        /// Adds a new page of the given type.
        /// </summary>
        /// <param name="type">The page type id</param>
        /// <param name="siteId">The optional site id</param>
        [Route("manager/page/add/{type}/{siteId:Guid?}")]
        [Authorize(Policy = Permission.PagesAdd)]
        public IActionResult Add(string type, Guid? siteId = null) {
            var sitemap = api.Sites.GetSitemap(siteId, onlyPublished: false);
            var model = PageEditModel.Create(api, type, siteId);
            model.SortOrder = sitemap.Count;

            return View("Edit", model);
        }

        /// <summary>
        /// Adds a new page of the given type at the specified position.
        /// </summary>
        /// <param name="type">The page type id</param>
        /// <param name="sortOrder">The sort order</param>
        /// <param name="parentId">The parent id</param>
        /// <param name="siteId">The optional site id</param>
        [Route("manager/page/add/{type}/{sortOrder:int}/{parentId:Guid?}/{siteId:Guid?}")]
        public IActionResult AddAt(string type, int sortOrder, Guid? parentId = null, Guid? siteId = null) {
            var model = PageEditModel.Create(api, type, siteId);

            model.ParentId = parentId;
            model.SortOrder = sortOrder;

            return View("Edit", model);
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        [Route("manager/page/save")]
        [Authorize(Policy = Permission.PagesSave)]
        public IActionResult Save(PageEditModel model) {
            // Validate
            if (string.IsNullOrWhiteSpace(model.Title)) {
                ErrorMessage("The page could not be saved. Title is mandatory", false);
                return View("Edit", model.Refresh(api));                
            }

            // Save
            if (model.Save(api)) {
                SuccessMessage("The page has been saved.");
                return RedirectToAction("Edit", new { id = model.Id });
            }
            ErrorMessage("The page could not be saved.", false);
            return View("Edit", model.Refresh(api));
        }

        /// <summary>
        /// Saves and publishes the given page model.
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        [Route("manager/page/publish")]
        [Authorize(Policy = Permission.PagesPublish)]
        public IActionResult Publish(PageEditModel model) {
            // Validate
            if (string.IsNullOrWhiteSpace(model.Title)) {
                ErrorMessage("The page could not be saved. Title is mandatory", false);
                return View("Edit", model.Refresh(api));                
            }

            // Save
            if (model.Save(api, true)) {
                SuccessMessage("The page has been published.");
                return RedirectToAction("Edit", new { id = model.Id });
            }
            ErrorMessage("The page could not be published.", false);
            return View(model);
        }

        /// <summary>
        /// Saves and unpublishes the given page model.
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        [Route("manager/page/unpublish")]
        [Authorize(Policy = Permission.PagesPublish)]
        public IActionResult UnPublish(PageEditModel model) {
            if (model.Save(api, false)) {
                SuccessMessage("The page has been unpublished.");
                return RedirectToAction("Edit", new { id = model.Id });
            }
            ErrorMessage("The page could not be unpublished.", false);
            return View(model);
        }

        /// <summary>
        /// Moves a page to match the given structure.
        /// </summary>
        /// <param name="structure">The page structure</param>
        [HttpPost]
        [Route("manager/pages/move")]
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult Move([FromBody]PageStructureModel structure) {
            for (var n = 0; n < structure.Items.Count; n++) {
                var moved = MovePage(structure.Items[n], n);
                if (moved)
                    break;
            }
            using (var config = new Config(api)) {
                return View("Partial/_Sitemap", new SitemapModel
                {
                    Sitemap = api.Sites.GetSitemap(onlyPublished: false),
                    ExpandedLevels = config.ManagerExpandedSitemapLevels
                });
            }
        }

        /// <summary>
        /// Deletes the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        [Route("manager/page/delete/{id:Guid}")]
        [Authorize(Policy = Permission.PagesDelete)]
        public IActionResult Delete(Guid id) {
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
        public IActionResult AddRegion([FromBody]PageRegionModel model) {
            var pageType = api.PageTypes.GetById(model.PageTypeId);

            if (pageType != null) {
                var regionType = pageType.Regions.SingleOrDefault(r => r.Id == model.RegionTypeId);

                if (regionType != null) {
                    var region = DynamicPage.CreateRegion(api,
                        model.PageTypeId, model.RegionTypeId);

                    var editModel = (PageEditRegionCollection)PageEditModel.CreateRegion(regionType, 
                        new List<object> { region});

                    ViewData.TemplateInfo.HtmlFieldPrefix = $"Regions[{model.RegionIndex}].FieldSets[{model.ItemIndex}]";
                    return View("EditorTemplates/PageEditRegionItem", editModel.FieldSets[0]);
                }
            }
            return new NotFoundResult();
        }

        /// <summary>
        /// Gets the page modal for the specified site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        [Route("manager/page/modal/{siteId:Guid?}")]
        [Authorize(Policy = Permission.Pages)]
        public IActionResult Modal(Guid? siteId = null) {
            return View(PageModalModel.GetBySiteId(api, siteId));
        }  
        

        #region Private methods
        private bool MovePage(PageStructureModel.PageStructureItem page, int sortOrder = 1, Guid? parentId = null) {
            var model = api.Pages.GetById(new Guid(page.Id));

            if (model != null) {
                if (model.ParentId != parentId || model.SortOrder != sortOrder) {
                    // Move the page in the structure.
                    api.Pages.Move(model, parentId, sortOrder);

                    // We only move one page at a time so we're done
                    return true;
                }

                for (var n = 0; n < page.Children.Count; n++) {
                    var moved = MovePage(page.Children[n], n, new Guid(page.Id));

                    if (moved)
                        return true;
                }
            }
            return false;
        }
        #endregion
    }
}
