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
using Piranha.Manager.Services;
using Piranha.Models;

namespace Piranha.Manager.Controllers;

[Area("Manager")]
[Route("manager/api/comments/moderation")]
[Authorize(Policy = Permission.Admin)]
[ApiController]
[AutoValidateAntiforgeryToken]
public class CommentModerationController : Controller
{
    private readonly IApi _api;
    private readonly CommentService _service;

    public CommentModerationController(IApi api, CommentService service)
    {
        _api = api;
        _service = service;
    }

    [HttpGet("pending/{siteId}")]
    [Authorize(Policy = Permission.Comments)]
    public async Task<IActionResult> GetPendingComments(Guid siteId)
    {
        var pages = await _api.Pages.GetAllAsync(siteId);
        var pending = new List<object>();

        foreach (var page in pages)
        {
            var comments = await _api.Pages.GetAllCommentsAsync(page.Id, onlyApproved: false, pageSize: 0);
            foreach (var comment in comments.Where(c => !c.IsApproved))
            {
                pending.Add(new
                {
                    comment.Id,
                    comment.Author,
                    BodyPreview = comment.Body.Length > 100
                        ? comment.Body.Substring(0, 100) + "..."
                        : comment.Body,
                    comment.Created,
                    PageTitle = page.Title,
                    PageId = page.Id
                });
            }
        }

        return Ok(new { siteId, comments = pending.OrderByDescending(c => ((dynamic)c).Created) });
    }

    [HttpGet("detail/{commentId}")]
    [Authorize(Policy = Permission.Comments)]
    public async Task<IActionResult> GetCommentDetail(Guid commentId)
    {
        var comment = await _api.Pages.GetCommentByIdAsync(commentId);
        if (comment == null)
        {
            comment = await _api.Posts.GetCommentByIdAsync(commentId);
        }

        if (comment == null)
        {
            return NotFound();
        }

        return Ok(new
        {
            comment.Id,
            comment.ContentId,
            comment.Author,
            comment.Email,
            comment.Url,
            comment.Body,
            comment.IsApproved,
            comment.Created
        });
    }

    [HttpPost("approve/{commentId}")]
    [Authorize(Policy = Permission.CommentsApprove)]
    public async Task<IActionResult> ApproveComment(Guid commentId)
    {
        var comment = await _api.Pages.GetCommentByIdAsync(commentId);
        if (comment == null)
        {
            comment = await _api.Posts.GetCommentByIdAsync(commentId);
        }

        if (comment == null)
        {
            return NotFound();
        }

        comment.IsApproved = true;

        if (comment is PageComment pageComment)
        {
            await _api.Pages.SaveCommentAsync(comment.ContentId, pageComment);
        }
        else if (comment is PostComment postComment)
        {
            await _api.Posts.SaveCommentAsync(comment.ContentId, postComment);
        }

        return Ok(new { status = "approved", commentId });
    }

    [HttpPost("reject/{commentId}")]
    [Authorize(Policy = Permission.CommentsApprove)]
    public async Task<IActionResult> RejectComment(Guid commentId)
    {
        var comment = await _api.Pages.GetCommentByIdAsync(commentId);
        if (comment == null)
        {
            comment = await _api.Posts.GetCommentByIdAsync(commentId);
        }

        if (comment == null)
        {
            return NotFound();
        }

        comment.IsApproved = false;

        if (comment is PageComment pageComment)
        {
            await _api.Pages.SaveCommentAsync(comment.ContentId, pageComment);
        }
        else if (comment is PostComment postComment)
        {
            await _api.Posts.SaveCommentAsync(comment.ContentId, postComment);
        }

        return Ok(new { status = "rejected", commentId });
    }
}
