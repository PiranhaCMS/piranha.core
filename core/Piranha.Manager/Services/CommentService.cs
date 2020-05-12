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
using System.Linq;
using System.Threading.Tasks;
using Piranha.Manager.Models;
using Piranha.Models;

namespace Piranha.Manager.Services
{
    public class CommentService
    {
        private readonly IApi _api;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public CommentService(IApi api)
        {
            _api = api;
        }

        /// <summary>
        /// Gets the comment model.
        /// </summary>
        /// <param name="id">The optional content id</param>
        /// <returns>The model</returns>
        public async Task<CommentListModel> Get(Guid? id = null)
        {
            var model = new CommentListModel
            {
                ContentId = id
            };

            var postComments = await _api.Posts.GetAllCommentsAsync(id, onlyApproved: false, pageSize: 0);
            var pageComments = await _api.Pages.GetAllCommentsAsync(id, onlyApproved: false, pageSize: 0);

            foreach (var postComment in postComments)
            {
                var post = await _api.Posts.GetByIdAsync<PostInfo>(postComment.ContentId);

                model.Comments.Add(new CommentListModel.ListItem
                {
                    Id = postComment.Id,
                    ArticleTitle = post?.Title,
                    ArticleUrl = $"manager/post/edit/{ post?.Id }",
                    Author = postComment.Author,
                    AuthorImage = Utils.GenerateGravatarUrl(postComment.Email, 25),
                    Email = postComment.Email,
                    Body = postComment.Body,
                    IsApproved = postComment.IsApproved,
                    Created = postComment.Created.ToString("yyyy-MM-dd"),
                    CreatedDate = postComment.Created
                });
            }

            foreach (var pageComment in pageComments)
            {
                var page = await _api.Pages.GetByIdAsync<PageInfo>(pageComment.ContentId);

                model.Comments.Add(new CommentListModel.ListItem
                {
                    Id = pageComment.Id,
                    ArticleTitle = page?.Title,
                    ArticleUrl = $"manager/page/edit/{ page?.Id }",
                    Author = pageComment.Author,
                    AuthorImage = Utils.GenerateGravatarUrl(pageComment.Email, 25),
                    Email = pageComment.Email,
                    Body = pageComment.Body,
                    IsApproved = pageComment.IsApproved,
                    Created = pageComment.Created.ToString("yyyy-MM-dd"),
                    CreatedDate = pageComment.Created
                });
            }

            model.Comments = model.Comments
                .OrderByDescending(c => c.CreatedDate)
                .ToList();

            return model;
        }

        public async Task ApproveAsync(Guid id)
        {
            var comment = await _api.Posts.GetCommentByIdAsync(id);

            if (comment != null)
            {
                comment.IsApproved = true;
                await _api.Posts.SaveCommentAsync(comment.ContentId, comment);
            }
            else
            {
                comment = await _api.Pages.GetCommentByIdAsync(id);

                if (comment != null)
                {
                    comment.IsApproved = true;
                    await _api.Pages.SaveCommentAsync(comment.ContentId, comment);
                }
            }
        }

        public async Task UnApproveAsync(Guid id)
        {
            var comment = await _api.Posts.GetCommentByIdAsync(id);

            if (comment != null)
            {
                comment.IsApproved = false;
                await _api.Posts.SaveCommentAsync(comment.ContentId, comment);
            }
            else
            {
                comment = await _api.Pages.GetCommentByIdAsync(id);

                if (comment != null)
                {
                    comment.IsApproved = false;
                    await _api.Pages.SaveCommentAsync(comment.ContentId, comment);
                }
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            var comment = await _api.Posts.GetCommentByIdAsync(id);

            if (comment != null)
            {
                await _api.Posts.DeleteCommentAsync(id);
            }
            else
            {
                comment = await _api.Pages.GetCommentByIdAsync(id);

                if (comment != null)
                {
                    await _api.Pages.DeleteCommentAsync(id);
                }
            }
        }
    }
}