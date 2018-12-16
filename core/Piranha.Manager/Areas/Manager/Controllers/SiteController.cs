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
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Areas.Manager.Models;
using Piranha.Areas.Manager.Services;
using Piranha.Manager;
using Piranha.Services;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class SiteController : ManagerAreaControllerBase
    {
        SiteContentEditService _service;
        IContentService<Data.Site, Data.SiteField, Piranha.Models.SiteContentBase> _contentService;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public SiteController(IApi api, SiteContentEditService service, IContentServiceFactory factory) : base(api)
        {
            _service = service;
            _contentService = factory.CreateSiteService();
        }

        /// <summary>
        /// Gets the list view for the current sites.
        /// </summary>
        [Route("manager/sites")]
        [Authorize(Policy = Permission.Sites)]
        public IActionResult List()
        {
            return View(SiteListModel.Get(_api));
        }

        [Route("manager/site/add")]
        [Authorize(Policy = Permission.SitesAdd)]
        public IActionResult Add()
        {
            return View("Edit", SiteEditModel.Create(_api));
        }

        [Route("manager/site/{id:Guid}")]
        [Authorize(Policy = Permission.SitesEdit)]
        public IActionResult Edit(Guid id)
        {
            return View(SiteEditModel.GetById(_api, id));
        }

        [Route("manager/site/save")]
        [Authorize(Policy = Permission.SitesSave)]
        public IActionResult Save(SiteEditModel model)
        {
            try
            {
                if (model.Save(_api))
                {
                    SuccessMessage("The site has been saved.");
                    return RedirectToAction("Edit", new { id = model.Site.Id });
                }
                else
                {
                    ErrorMessage("The site could not be saved.", false);
                    return View("Edit", model);
                }
            }
            catch (ArgumentException)
            {
                ErrorMessage("The site could not be saved. Title is mandatory", false);
                return View("Edit", model);
            }
        }

        [Route("manager/site/delete/{id:Guid}")]
        [Authorize(Policy = Permission.SitesDelete)]
        public IActionResult Delete(Guid id)
        {
            var site = _api.Sites.GetById(id);

            if (site != null)
            {
                if (!site.IsDefault)
                {
                    _api.Sites.Delete(id);
                    SuccessMessage("The site has been deleted.");
                }
                else
                {
                    ErrorMessage("Can't delete the default site.");
                }
            }
            else
            {
                ErrorMessage("This site could not be found.");
            }
            return RedirectToAction("List");
        }

        [Route("manager/site/content/{id:Guid}")]
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult EditContent(Guid id)
        {
            return View("EditContent", _service.GetById(id));
        }

        [HttpPost]
        [Route("manager/site/content/save")]
        [Authorize(Policy = Permission.PagesEdit)]
        public IActionResult SaveContent(SiteContentEditModel model)
        {
            if (_service.Save(model))
            {
                SuccessMessage("The site content has been saved.");
                return RedirectToAction("EditContent", new { id = model.Id });
            }
            else
            {
                ErrorMessage("The site content could not be saved.", false);
                return View("EditContent", _service.Refresh(model));
            }
        }

        /// <summary>
        /// Adds a new region to a site.
        /// </summary>
        /// <param name="model">The model</param>
        [HttpPost]
        [Route("manager/site/region")]
        [Authorize(Policy = Permission.Posts)]
        public IActionResult AddRegion([FromBody]PageRegionModel model)
        {
            var siteType = _api.SiteTypes.GetById(model.PageTypeId);

            if (siteType != null)
            {
                var regionType = siteType.Regions.SingleOrDefault(r => r.Id == model.RegionTypeId);

                if (regionType != null)
                {
                    var region = _contentService.CreateDynamicRegion(siteType, model.RegionTypeId);

                    var editModel = (PageEditRegionCollection)_service.CreateRegion(regionType,
                        new List<object> { region });

                    ViewData.TemplateInfo.HtmlFieldPrefix = $"Regions[{model.RegionIndex}].FieldSets[{model.ItemIndex}]";
                    return View("EditorTemplates/PageEditRegionItem", editModel.FieldSets[0]);
                }
            }
            return new NotFoundResult();
        }
    }
}
