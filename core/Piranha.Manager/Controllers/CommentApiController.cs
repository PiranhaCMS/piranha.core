/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;
using Piranha.Manager.Services;

namespace Piranha.Manager.Controllers;

/// <summary>
/// Api controller for comment management.
/// </summary>
[Area("Manager")]
[Route("manager/api/comment")]
[Authorize(Policy = Permission.Admin)]
[ApiController]
[AutoValidateAntiforgeryToken]
public class CommentApiController : Controller
{
    public class ApprovalModel
    {
        public Guid Id { get; set; }
        public Guid? ParentId { get; set; }
    }

    private readonly CommentService _service;
    private readonly ManagerLocalizer _localizer;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public CommentApiController(CommentService service, ManagerLocalizer localizer)
    {
        _service = service;
        _localizer = localizer;
    }

    /// <summary>
    /// Gets the list model.
    /// </summary>
    /// <returns>The list model</returns>
    [Route("{id:Guid?}")]
    [HttpGet]
    [Authorize(Policy = Permission.Comments)]
    public Task<CommentListModel> List(Guid? id = null)
    {
        return _service.Get(id);
    }

    [Route("approve")]
    [HttpPost]
    [Authorize(Policy = Permission.CommentsApprove)]
    public async Task<CommentListModel> Approve(ApprovalModel model)
    {
        await _service.ApproveAsync(model.Id).ConfigureAwait(false);

        var result = await List(model.ParentId).ConfigureAwait(false);

        result.Status = new StatusMessage
        {
            Type = StatusMessage.Success,
            Body = _localizer.Comment["The comment was successfully approved"]
        };
        return result;
    }

    [Route("unapprove")]
    [HttpPost]
    [Authorize(Policy = Permission.CommentsApprove)]
    public async Task<CommentListModel> UnApprove(ApprovalModel model)
    {
        await _service.UnApproveAsync(model.Id).ConfigureAwait(false);

        var result = await List(model.ParentId).ConfigureAwait(false);

        result.Status = new StatusMessage
        {
            Type = StatusMessage.Success,
            Body = _localizer.Comment["The comment was successfully unapproved"]
        };
        return result;
    }

    [Route("delete")]
    [HttpDelete]
    [Authorize(Policy = Permission.CommentsDelete)]
    public async Task<StatusMessage> Delete([FromBody]Guid id)
    {
        await _service.DeleteAsync(id).ConfigureAwait(false);

        var result = new StatusMessage
        {
            Type = StatusMessage.Success,
            Body = _localizer.Comment["The comment was successfully deleted"]
        };
        return result;
    }
}
