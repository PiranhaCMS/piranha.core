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

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Config model.
    /// </summary>
    public static class ContentState
    {
        public static string New = "new";
        public static string Unpublished = "unpublished";
        public static string Published = "published";
        public static string Draft = "draft";
    }
}