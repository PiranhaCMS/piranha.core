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
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for page management.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/page")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class PageApiController : Controller
    {
        private readonly PageService _service;
        private readonly IApi _api;
        private readonly ManagerLocalizer _localizer;
        private readonly IHubContext<Hubs.PreviewHub> _hub;
        private readonly IAuthorizationService _auth;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PageApiController(PageService service, IApi api, ManagerLocalizer localizer, IHubContext<Hubs.PreviewHub> hub, IAuthorizationService auth)
        {
            _service = service;
            _api = api;
            _localizer = localizer;
            _hub = hub;
            _auth = auth;
        }

        /// <summary>
        /// Gets the list model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("list")]
        [HttpGet]
        [Authorize(Policy = Permission.Pages)]
        public async Task<PageListModel> List()
        {
            var model = await _service.GetList();

            return model;
        }

        /// <summary>
        /// Gets the sitemap model.
        /// </summary>
        /// <returns>The list model</returns>
        [Route("sitemap/{siteId?}")]
        [HttpGet]
        public async Task<SiteListModel> Sitemap(Guid? siteId = null)
        {
            if (!siteId.HasValue)
            {
                siteId = (await _api.Sites.GetDefaultAsync()).Id;
            }
            return await _service.GetSiteList(siteId.Value);
            //return await _service.GetPageStructure(siteId.Value);
        }

        /// <summary>
        /// Gets the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The page edit model</returns>
        [Route("{id:Guid}")]
        [HttpGet]
        [Authorize(Policy = Permission.PagesEdit)]
        public async Task<PageEditModel> Get(Guid id)
        {
            return await _service.GetById(id);
        }

        /// <summary>
        /// Creates a new page of the specified type.
        /// </summary>
        /// <param name="siteId">The site id</param>
        /// <param name="typeId">The type id</param>
        /// <returns>The page edit model</returns>
        [Route("create/{siteId}/{typeId}")]
        [HttpGet]
        [Authorize(Policy = Permission.PagesAdd)]
        public async Task<PageEditModel> Create(Guid siteId, string typeId)
        {
            return await _service.Create(siteId, typeId);
        }

        /// <summary>
        /// Creates a new page of the specified type.
        /// </summary>
        /// <param name="pageId">The page the new page should be position relative to</param>
        /// <param name="typeId">The type id</param>
        /// <param name="after">If the new page should be positioned after the existing page</param>
        /// <returns>The page edit model</returns>
        [Route("createrelative/{pageId}/{typeId}/{after}")]
        [HttpGet]
        [Authorize(Policy = Permission.PagesAdd)]
        public async Task<PageEditModel> CreateRelative(Guid pageId, string typeId, bool after)
        {
            return await _service.CreateRelative(pageId, typeId, after);
        }

        /// <summary>
        /// Creates a new page of the specified type.
        /// </summary>
        /// <param name="sourceId">The page that should be copied</param>
        /// <param name="pageId">The page the new page should be position relative to</param>
        /// <param name="after">If the new page should be positioned after the existing page</param>
        /// <returns>The page edit model</returns>
        [Route("copyrelative/{sourceId}/{pageId}/{after}")]
        [HttpGet]
        [Authorize(Policy = Permission.PagesAdd)]
        public async Task<PageEditModel> CopyRelative(Guid sourceId, Guid pageId, bool after)
        {
            return await _service.CopyRelative(sourceId, pageId, after);
        }

        /// <summary>
        /// Detaches the given copy into a unique page instance.
        /// </summary>
        /// <param name="pageId">The page id</param>
        /// <returns>The page edit model</returns>
        [Route("detach/{pageId}")]
        [HttpGet]
        [Authorize(Policy = Permission.PagesEdit)]
        public async Task<PageEditModel> Detach(Guid pageId)
        {
            var model = await _service.Detach(pageId);

            if (model != null)
            {
                model.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = _localizer.Page["The page was successfully detached from the original"]
                };
                return model;
            }
            return null;
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save")]
        [HttpPost]
        [Authorize(Policy = Permission.PagesPublish)]
        public async Task<PageEditModel> Save(PageEditModel model)
        {
            // Ensure that we have a published date
            if (string.IsNullOrEmpty(model.Published))
            {
                model.Published = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            }

            var ret = await Save(model, false);
            await _hub?.Clients.All.SendAsync("Update", model.Id);

            return ret;
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save/draft")]
        [HttpPost]
        [Authorize(Policy = Permission.PagesSave)]
        public async Task<PageEditModel> SaveDraft(PageEditModel model)
        {
            var ret = await Save(model, true);
            await _hub?.Clients.All.SendAsync("Update", model.Id);

            return ret;
        }

        /// <summary>
        /// Saves the given model and unpublishes it
        /// </summary>
        /// <param name="model">The model</param>
        /// <returns>The result of the operation</returns>
        [Route("save/unpublish")]
        [HttpPost]
        [Authorize(Policy = Permission.PagesPublish)]
        public async Task<PageEditModel> SaveUnpublish(PageEditModel model)
        {
            // Remove published date
            model.Published = null;

            var ret = await Save(model, false);
            await _hub?.Clients.All.SendAsync("Update", model.Id);

            return ret;
        }

        [Route("revert/{id}")]
        [HttpGet]
        [Authorize(Policy = Permission.PagesSave)]
        public async Task<PageEditModel> Revert(Guid id)
        {
            var page = await _service.GetById(id, false);

            if (page != null)
            {
                await _service.Save(page, false);

                page = await _service.GetById(id);
            }

            page.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The page was successfully reverted to its previous state"]
            };

            await _hub?.Clients.All.SendAsync("Update", id);

            return page;
        }

        /// <summary>
        /// Deletes the page with the given id.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <returns>The result of the operation</returns>
        [Route("delete/{id}")]
        [HttpGet]
        [Authorize(Policy = Permission.PagesDelete)]
        public async Task<StatusMessage> Delete(Guid id)
        {
            try
            {
                await _service.Delete(id);
            }
            catch (ValidationException e)
            {
                // Validation did not succeed
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };
            }
            catch
            {
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = _localizer.Page["An error occured while deleting the page"]
                };
            }

            return new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = _localizer.Page["The page was successfully deleted"]
            };
        }

        [Route("move")]
        [HttpPost]
        [Authorize(Policy = Permission.PagesEdit)]
        public async Task<PageListModel> Move([FromBody]StructureModel model)
        {
            if (await _service.MovePages(model))
            {
                var list = await List();

                list.Status = new StatusMessage
                {
                    Type = StatusMessage.Success,
                    Body = _localizer.Page["The sitemap was successfully updated"]
                };
                return list;
            }
            return new PageListModel {
                Status = new StatusMessage
                {
                    Type = StatusMessage.Warning,
                    Body = _localizer.Page["No pages changed position"]
                }
            };
        }

        /// <summary>
        /// Saves the given model
        /// </summary>
        /// <param name="model">The model</param>
        /// <param name="draft">If the page should be saved as a draft</param>
        /// <returns>The result of the operation</returns>
        private async Task<PageEditModel> Save(PageEditModel model, bool draft = false)
        {
            try
            {
                await _service.Save(model, draft);
            }
            catch (ValidationException e)
            {
                model.Status = new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = e.Message
                };

                return model;
            }
            /*
            catch
            {
                return new StatusMessage
                {
                    Type = StatusMessage.Error,
                    Body = "An error occured while saving the page"
                };
            }
            */

            var ret = await _service.GetById(model.Id);
            ret.Status = new StatusMessage
            {
                Type = StatusMessage.Success,
                Body = draft ? _localizer.Page["The page was successfully saved"]
                    : string.IsNullOrEmpty(model.Published) ? _localizer.Page["The page was successfully unpublished"] : _localizer.Page["The page was successfully published"]
            };

            return ret;
        }
    }
}