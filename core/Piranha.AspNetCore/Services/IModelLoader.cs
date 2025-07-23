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
using Piranha.Models;

namespace Piranha.AspNetCore.Services;

/// <summary>
/// The model loader is used for retrieving content models with
/// built in permission checks for the current user.
/// </summary>
public interface IModelLoader
{
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
    Task<T> GetPageAsync<T>(Guid id, ClaimsPrincipal user, bool draft = false) where T : PageBase;

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
    Task<T> GetPostAsync<T>(Guid id, ClaimsPrincipal user, bool draft = false) where T : PostBase;
}
