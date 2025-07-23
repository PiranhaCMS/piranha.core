/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Mvc;

namespace Piranha.Manager;

public sealed class ManagerOptions
{
    /// <summary>
    /// Gets/sets the XSRF cookie name that will be set by the manager after
    /// successful authentication. The default is "XSRF-REQUEST-TOKEN".
    /// </summary>
    public string XsrfCookieName { get; set; } = "XSRF-REQUEST-TOKEN";

    /// <summary>
    /// Gets/sets the name of the header the manager will use to send back the
    /// anti forgery information to the API. The default is "X-XSRF-TOKEN".
    /// </summary>
    public string XsrfHeaderName { get; set; } = "X-XSRF-TOKEN";

    /// <summary>
    /// Gets/sets the optional JSON options for the Newtonsoft serializer.
    /// </summary>
    /// <value></value>
    public Action<MvcNewtonsoftJsonOptions> JsonOptions { get; set; }
}
