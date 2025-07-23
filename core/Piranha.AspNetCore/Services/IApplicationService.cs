/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Http;
using Piranha.AspNetCore.Helpers;
using Piranha.Models;

namespace Piranha.AspNetCore.Services;

/// <summary>
/// The main application service. This service must be
/// registered as a scoped service as it contains information
/// about the current requst.
/// </summary>
public interface IApplicationService
{
    /// <summary>
    /// Gets the current api.
    /// </summary>
    IApi Api { get; }

    /// <summary>
    /// Gets the site helper.
    /// </summary>
    ISiteHelper Site { get; }

    /// <summary>
    /// Gets the media helper.
    /// </summary>
    IMediaHelper Media { get; }

    /// <summary>
    /// Gets the request helper.
    /// </summary>
    IRequestHelper Request { get; }

    /// <summary>
    /// Gets/sets the id of the currently requested page.
    /// </summary>
    Guid PageId { get; set; }

    /// <summary>
    /// Gets/sets the current page.
    /// </summary>
    PageBase CurrentPage { get; set; }

    /// <summary>
    /// Gets/sets the current post.
    /// </summary>
    PostBase CurrentPost { get; set; }

    /// <summary>
    /// Initializes the service.
    /// </summary>
    Task InitAsync(HttpContext context);

    /// <summary>
    /// Gets the gravatar URL from the given parameters.
    /// </summary>
    /// <param name="email">The email address</param>
    /// <param name="size">The requested size</param>
    /// <returns>The gravatar URL</returns>
    string GetGravatarUrl(string email, int size = 0);
}
