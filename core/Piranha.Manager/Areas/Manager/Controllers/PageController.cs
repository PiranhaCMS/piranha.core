/*
 * Copyright (c) 2016-2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Piranha.Areas.Manager.Services;
using Piranha.Manager;
using Piranha.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class PageController : ManagerAreaControllerBase
    {
        private const string COOKIE_SELECTEDSITE = "PiranhaManager_SelectedSite";
        private readonly PageEditService editService;
        private readonly IContentService<Data.Page, Data.PageField, Piranha.Models.PageBase> contentService;
        private readonly IHubContext<Hubs.PreviewHub> _hub;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="editService">The current page edit service</param>
        /// <param name="factory">The content service factory</param>
        public PageController(IApi api, PageEditService editService, IContentServiceFactory factory, IHubContext<Hubs.PreviewHub> hub) : base(api) { 
            this.editService = editService;
            this.contentService = factory.CreatePageService();
            _hub = hub;
        }

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
            var model = Models.PageListModel.Get(api, siteId, pageId);
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
            return View(editService.GetById(id));
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
            var model = editService.Create(type, siteId);
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
        [Route("manager/page/add/{type}/{sortOrder:int}/{parentId:Guid?}")]
        [Authorize(Policy = Permission.PagesAdd)]
        public IActionResult AddAt(string type, int sortOrder, Guid? parentId = null) {
            return AddAt(Guid.Empty, type, sortOrder, parentId);
        }

        /// <summary>
        /// Adds a new page of the given type at the specified position.
        /// </summary>
        /// <param name="type">The page type id</param>
        /// <param name="sortOrder">The sort order</param>
        /// <param name="parentId">The parent id</param>
        /// <param name="siteId">The optional site id</param>
        [Route("manager/page/add/site/{siteId:Guid}/{type}/{sortOrder:int}/{parentId:Guid?}")]
        [Authorize(Policy = Permission.PagesAdd)]
        public IActionResult AddAt(Guid siteId, string type, int sortOrder, Guid? parentId = null) {
            var model = editService.Create(type, siteId != Guid.Empty ? siteId : (Guid?)null);

            model.ParentId = parentId;
            model.SortOrder = sortOrder;

            return View("Edit", model);
        }        

        /// <summary>
        /// Adds a new copy of the specified page.
        /// </summary>
        /// <param name="id">The page to copy</param>
        /// <param name="siteId">The optional site id</param>
        [Route("manager/page/copy/{originalId:Guid}/{siteId:Guid?}")]
        [Authorize(Policy = Permission.PagesAdd)]
        public IActionResult AddCopy(Guid originalId, Guid? siteId = null) {
            if (!siteId.HasValue) {
                var site = api.Sites.GetDefault();
                siteId = site.Id;
            }

            var sitemap = api.Sites.GetSitemap(siteId, onlyPublished: false);
            var model = editService.GetById(originalId);

            if (model.OriginalPageId.HasValue) {
                ErrorMessage("Can't create a copy from a copied page.");
                return RedirectToAction("List", new { siteId = siteId.Value });
            }

            model.OriginalPageId = originalId;
            model.Id = Guid.NewGuid();
            model.SiteId = siteId.Value;
            model.ParentId = null;
            model.SortOrder = sitemap.Count;     
            model.Title = "Copy of " + model.Title;
            model.NavigationTitle = "";
            model.Created = model.LastModified = DateTime.MinValue;
            model.Published = null;
            model.Slug = null;

            return View("Edit", model);
        }

        [Route("manager/page/preview/{id:Guid}")]
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult Preview(Guid id) {
            var page = api.Pages.GetById<Piranha.Models.PageInfo>(id);

            if (page != null)
                return View("_Preview", new Models.PreviewModel { Id = id, Permalink = page.Permalink });
            return NotFound();
        }

        /// <summary>
        /// Detaches a copied page.
        /// </summary>
        /// <param name="id">The page id</param>
        [Route("manager/page/detach/{id:Guid}")]
        [Authorize(Policy = Permission.PagesSave)]
        public IActionResult Detach(Guid id) {
            var page = api.Pages.GetById(id);

            if (page != null && page.OriginalPageId.HasValue) {
                api.Pages.Detach(page);

                SuccessMessage("The page has been detached from its original.");
                return RedirectToAction("Edit", new { id = id});
            } else {
                ErrorMessage("The page could not be be detached.");
                return RedirectToAction("Edit", new { id = id});                
            }
        }

        /// <summary>
        /// Saves the given page model
        /// </summary>
        /// <param name="model">The page model</param>
        [HttpPost]
        [Route("manager/page/save")]
        [Authorize(Policy = Permission.PagesSave)]
        public async Task<IActionResult> Save(Models.PageEditModel model) {
            // Validate
            if (string.IsNullOrWhiteSpace(model.Title)) {
                return BadRequest();
            }

            var ret = editService.Save(model, out var alias);

            // Save
            if (ret) {
                if (_hub != null)
                    await _hub.Clients.All.SendAsync("Update", model.Id);

                if (!string.IsNullOrWhiteSpace(alias))
                    return Json(new
                    {
                        Location = Url.Action("Edit", new { id = model.Id }),
                        AliasSuggestion = new
                        {
                            Alias = alias,
                            Redirect = model.Slug,
                            SiteId = model.SiteId,
                            PageId = model.Id
                        }   
                    });
                else
                    return Json(new { Location = Url.Action("Edit", new { id = model.Id }) });
            } else {
                return StatusCode(500);
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
            // Validate
            if (string.IsNullOrWhiteSpace(model.Title)) {
                return BadRequest();
            }

            // Save
            if (editService.Save(model, out var alias, true)) {
                return Json(new
                {
                    Published = DateTime.Now.ToString("yyyy-MM-dd HH:mm"),
                    Location = Url.Action("Edit", new { id = model.Id })
                });
            } else {
                return StatusCode(500);
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
            if (editService.Save(model, out var alias, false)) {
                return Json(new
                {
                    Unpublished = true,
                    Location = Url.Action("Edit", new { id = model.Id })
                });
            } else {
                return StatusCode(500);
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
            Guid? siteId = null;

            if (structure.Items.Count > 0)
            {
                var page = api.Pages.GetById(new Guid(structure.Items[0].Id));

                if (page != null)
                    siteId = page.SiteId;
            }

            for (var n = 0; n < structure.Items.Count; n++) {
                var moved = MovePage(structure.Items[n], n);
                if (moved)
                    break;
            }
            using (var config = new Config(api)) {
                return View("Partial/_Sitemap", new Models.SitemapModel() {
                    Sitemap = api.Sites.GetSitemap(siteId, onlyPublished: false),
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
        public IActionResult AddRegion([FromBody]Models.PageRegionModel model) {
            var pageType = api.PageTypes.GetById(model.PageTypeId);

            if (pageType != null) {
                var regionType = pageType.Regions.SingleOrDefault(r => r.Id == model.RegionTypeId);

                if (regionType != null) {
                    var region = contentService.CreateDynamicRegion(pageType, model.RegionTypeId);

                    var editModel = (Models.PageEditRegionCollection)editService.CreateRegion(regionType, 
                        new List<object>() { region});

                    ViewData.TemplateInfo.HtmlFieldPrefix = $"Regions[{model.RegionIndex}].FieldSets[{model.ItemIndex}]";
                    return View("EditorTemplates/PageEditRegionItem", editModel.FieldSets[0]);
                }
            }
            return new NotFoundResult();
        }

        [HttpPost]
        [Route("manager/page/alias")]
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult AddAlias(Guid siteId, Guid pageId, string alias, string redirect) {
            // Create alias
            Piranha.Manager.Utils.CreateAlias(api, siteId, alias, redirect);

            // Check if there are posts for this page
            var posts = api.Posts.GetAll(pageId);
            foreach (var post in posts) {
                // Only create aliases for published posts
                if (post.Published.HasValue)
                    Piranha.Manager.Utils.CreateAlias(api, siteId, $"{alias}/{post.Slug}", $"{redirect}/{post.Slug}");
            }

            return Json(new
            {
                PostAffected = posts.Count()
            });
        }

        /// <summary>
        /// Gets the page modal for the specified site.
        /// </summary>
        /// <param name="siteId">The site id</param>
        [Route("manager/page/modal/{siteId:Guid?}")]
        [Authorize(Policy = Permission.Pages)]
        public IActionResult Modal(Guid? siteId = null) {
            if (!siteId.HasValue)
            {
                var site = Request.Cookies[COOKIE_SELECTEDSITE];
                if (!string.IsNullOrEmpty(site))
                    siteId = new Guid(site);
            }
            return View(Models.PageModalModel.GetBySiteId(api, siteId));
        }  
        
        private bool MovePage(Models.PageStructureModel.PageStructureItem page, int sortOrder = 1, Guid? parentId = null) {
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
    }
}
