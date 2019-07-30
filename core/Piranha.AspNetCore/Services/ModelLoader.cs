/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Piranha.Models;

namespace Piranha.AspNetCore.Services
{
    public class ModelLoader : IModelLoader
    {
        protected readonly IApi _api;
        protected readonly IAuthorizationService _auth;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="api">The current api</param>
        public ModelLoader(IApi api, IAuthorizationService auth)
        {
            _api = api;
            _auth = auth;
        }

        /// <summary>
        /// Gets the specified page model for the given user.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="user">The current user</param>
        /// <param name="draft">If a draft should be loaded</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The page model</returns>
        public async Task<T> GetPage<T>(Guid id, ClaimsPrincipal user, bool draft = false)
            where T : PageBase
        {
            T model = null;

            // Check if we're requesting a draft
            if (draft)
            {
                // Check that the current user is authorized to preview pages
                if ((await _auth.AuthorizeAsync(user, Security.Permission.PagePreview)).Succeeded)
                {
                    // Get the draft, if available
                    model = await _api.Pages.GetDraftByIdAsync<T>(id);

                    if (model == null)
                    {
                        model = await _api.Pages.GetByIdAsync<T>(id);
                    }
                }
            }

            // No draft loaded or requested, try to get the published page
            if (model == null)
            {
                model = await _api.Pages.GetByIdAsync<T>(id);

                if (model != null)
                {
                    // Make sure the page is published
                    if (!model.Published.HasValue || model.Published.Value > DateTime.Now)
                    {
                        // No published version exists
                        return null;
                    }
                }
                else
                {
                    // No page found with the specified id
                    return null;
                }
            }
            return model;
        }

        /// <summary>
        /// Gets the specified post model for the given user.
        /// </summary>
        /// <param name="id">The unique id</param>
        /// <param name="user">The current user</param>
        /// <param name="draft">If a draft should be loaded</param>
        /// <typeparam name="T">The model type</typeparam>
        /// <returns>The post model</returns>
        public async Task<T> GetPost<T>(Guid id, ClaimsPrincipal user, bool draft = false)
            where T : PostBase
        {
            T model = null;

            // Check if we're requesting a draft
            if (draft)
            {
                // Check that the current user is authorized to preview pages
                if ((await _auth.AuthorizeAsync(user, Security.Permission.PostPreview)).Succeeded)
                {
                    // Get the draft, if available
                    model = await _api.Posts.GetDraftByIdAsync<T>(id);

                    if (model == null)
                    {
                        model = await _api.Posts.GetByIdAsync<T>(id);
                    }
                }
            }

            // No draft loaded or requested, try to get the published page
            if (model == null)
            {
                model = await _api.Posts.GetByIdAsync<T>(id);

                if (model != null)
                {
                    // Make sure the page is published
                    if (!model.Published.HasValue || model.Published.Value > DateTime.Now)
                    {
                        // No published version exists
                        return null;
                    }
                }
                else
                {
                    // No page found with the specified id
                    return null;
                }
            }
            return model;
        }
    }
}