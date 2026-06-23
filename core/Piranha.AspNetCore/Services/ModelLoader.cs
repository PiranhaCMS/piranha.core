/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Piranha.Models;

namespace Piranha.AspNetCore.Services;

/// <summary>
/// The model loader is used for retrieving content models with
/// built in permission checks for the current user.
/// </summary>
public class ModelLoader : IModelLoader
{
    /// <summary>
    /// The current api.
    /// </summary>
    protected readonly IApi _api;

    /// <summary>
    /// The current authorization service.
    /// </summary>
    protected readonly IAuthorizationService _auth;

    /// <summary>
    /// The current application service.
    /// </summary>
    protected readonly IApplicationService _app;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="api">The current api</param>
    /// <param name="auth">The authorization service</param>
    /// <param name="app">The application service</param>
    public ModelLoader(IApi api, IAuthorizationService auth, IApplicationService app)
    {
        _api = api;
        _auth = auth;
        _app = app;
    }

    /// <summary>
    /// Gets the specified page model for the given user. If the
    /// user doesn't have access to the requested page an
    /// UnauthorizedAccessException is thrown.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <param name="user">The current user</param>
    /// <param name="draft">If a draft should be loaded</param>
    /// <typeparam name="T">The model type</typeparam>
    /// <returns>The page model</returns>
    public async Task<T> GetPageAsync<T>(Guid id, ClaimsPrincipal user, bool draft = false)
        where T : PageBase
    {
        T model = null;

        if (draft)
        {
            // Draft access requires explicit preview permission — checked before any data load.
            if (!(await _auth.AuthorizeAsync(user, Piranha.Security.Permission.PagePreview)).Succeeded)
            {
                return null;
            }
            model = await _api.Pages.GetDraftByIdAsync<T>(id);
            if (model == null)
            {
                model = await _api.Pages.GetByIdAsync<T>(id);
            }
        }
        else
        {
            // Use the already-resolved page from the application service if available,
            // otherwise fetch from the database.
            if (_app.CurrentPage != null && _app.CurrentPage.Id == id && _app.CurrentPage is T)
            {
                model = (T)_app.CurrentPage;
            }
            else
            {
                model = await _api.Pages.GetByIdAsync<T>(id);
            }

            if (model == null)
            {
                return null;
            }

            // Always verify the published state for non-draft access, even for cache hits.
            if (!model.Published.HasValue || model.Published.Value > DateTime.Now)
            {
                return null;
            }
        }

        if (model == null)
        {
            return null;
        }

        // Check permissions
        if (model.Permissions.Count > 0)
        {
            var currentPermissions = new HashSet<string>(App.Permissions.GetPublicPermissions()
                .Select(p => p.Name));

            foreach (var permission in model.Permissions)
            {
                if (permission == null || !currentPermissions.Contains(permission))
                {
                    throw new UnauthorizedAccessException();
                }

                // Authorize
                if (!(await _auth.AuthorizeAsync(user, permission)).Succeeded)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
        return model;
    }

    /// <summary>
    /// Gets the specified post model for the given user. If the
    /// user doesn't have access to the requested post an
    /// UnauthorizedAccessException is thrown.
    /// </summary>
    /// <param name="id">The unique id</param>
    /// <param name="user">The current user</param>
    /// <param name="draft">If a draft should be loaded</param>
    /// <typeparam name="T">The model type</typeparam>
    /// <returns>The post model</returns>
    public async Task<T> GetPostAsync<T>(Guid id, ClaimsPrincipal user, bool draft = false)
        where T : PostBase
    {
        T model = null;

        if (draft)
        {
            // Draft access requires explicit preview permission — checked before any data load.
            if (!(await _auth.AuthorizeAsync(user, Piranha.Security.Permission.PostPreview)).Succeeded)
            {
                return null;
            }
            model = await _api.Posts.GetDraftByIdAsync<T>(id);
            if (model == null)
            {
                model = await _api.Posts.GetByIdAsync<T>(id);
            }
        }
        else
        {
            // Use the already-resolved post from the application service if available,
            // otherwise fetch from the database.
            if (_app.CurrentPost != null && _app.CurrentPost.Id == id && _app.CurrentPost is T)
            {
                model = (T)_app.CurrentPost;
            }
            else
            {
                model = await _api.Posts.GetByIdAsync<T>(id);
            }

            if (model == null)
            {
                return null;
            }

            // Always verify the published state for non-draft access, even for cache hits.
            if (!model.Published.HasValue || model.Published.Value > DateTime.Now)
            {
                return null;
            }
        }

        if (model == null)
        {
            return null;
        }

        // Check permissions
        if (model.Permissions.Count > 0)
        {
            var currentPermissions = new HashSet<string>(App.Permissions.GetPublicPermissions()
                .Select(p => p.Name));

            foreach (var permission in model.Permissions)
            {
                if (permission == null || !currentPermissions.Contains(permission))
                {
                    throw new UnauthorizedAccessException();
                }

                // Authorize
                if (!(await _auth.AuthorizeAsync(user, permission)).Succeeded)
                {
                    throw new UnauthorizedAccessException();
                }
            }
        }
        return model;
    }
}
