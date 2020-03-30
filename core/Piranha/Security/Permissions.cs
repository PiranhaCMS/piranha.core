/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

namespace Piranha.Security
{
    /// <summary>
    /// The available core permissions.
    /// </summary>
    public static class Permission
    {
        public const string PagePreview = "PiranhaPagePreview";
        public const string PostPreview = "PiranhaPostPreview";

        public static string[] All()
        {
            return new []
            {
                PagePreview,
                PostPreview
            };
        }
    }
}