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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Piranha.AspNetCore.Services;
using Piranha.Models;

namespace Piranha.AspNetCore.Models
{
    /// <summary>
    /// Razor Page model for a single page with comment helpers.
    /// </summary>
    /// <typeparam name="T">The page type</typeparam>
    public class SinglePageWithComments<T> : SinglePage<T> where T : PageBase
    {
        /// <summary>
        /// Gets/sets the available comments.
        /// </summary>
        public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();

        [BindProperty]
        public string CommentAuthor { get; set; }

        [BindProperty]
        public string CommentEmail { get; set; }

        [BindProperty]
        public string CommentUrl { get; set; }

        [BindProperty]
        public string CommentBody { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        /// <param name="loader">The model loader</param>
        public SinglePageWithComments(IApi api, IModelLoader loader) : base(api, loader) { }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <param name="id">The requested model id</param>
        /// <param name="draft">If the draft should be fetched</param>
        public override async Task<IActionResult> OnGet(Guid id, bool draft = false)
        {
            var result = await base.OnGet(id, draft);

            if (Data != null)
            {
                Comments = await _api.Pages.GetAllCommentsAsync(Data.Id);
            }
            return result;
        }

        /// <summary>
        /// Gets the model data.
        /// </summary>
        /// <param name="id">The requested model id</param>
        /// <param name="draft">If the draft should be fetched</param>
        public virtual async Task<IActionResult> OnPostSaveComment(Guid id, bool draft = false)
        {
            // Create the comment
            var comment = new Comment
            {
                IpAddress = Request.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = Request.Headers.ContainsKey("User-Agent") ? Request.Headers["User-Agent"].ToString() : "",
                Author = CommentAuthor,
                Email = CommentEmail,
                Url = CommentUrl,
                Body = CommentBody
            };

            await _api.Pages.SaveCommentAndVerifyAsync(id, comment);

            Data = await _loader.GetPageAsync<T>(id, HttpContext.User, draft);

            return Redirect(Data.Permalink + "#comments");
        }
    }
}