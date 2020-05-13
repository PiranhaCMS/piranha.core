/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Manager.Models
{
    /// <summary>
    /// The different states revision based content can have.
    /// </summary>
    public static class ContentState
    {
        public static string New { get; } = "new";
        public static string Unpublished { get; } = "unpublished";
        public static string Published { get; } = "published";
        public static string Draft { get; } = "draft";
    }
}