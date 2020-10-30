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

namespace Piranha.AttributeBuilder
{
    /// <summary>
    /// Attribute for adding a route to a post type.
    /// </summary>
    [NoCoverage]
    [Obsolete("PostTypeRouteAttribute is obsolete and has been replaced with ContentTypeRouteAttribute", true)]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PostTypeRouteAttribute : ContentTypeRouteAttribute { }
}
