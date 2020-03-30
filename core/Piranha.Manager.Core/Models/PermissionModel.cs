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

namespace Piranha.Manager.Models
{
    public class PermissionModel
    {
        public class AliasPermissions
        {
            public bool Delete { get; set; }
            public bool Edit { get; set; }
        }

        public class CommentPermissions
        {
            public bool Approve { get; set; }
            public bool Delete { get; set; }
        }

        public class MediaPermissions
        {
            public bool Add { get; set; }
            public bool AddFolder { get; set; }
            public bool Delete { get; set; }
            public bool DeleteFolder { get; set; }
            public bool Edit { get; set; }
        }

        public class PagePermissions
        {
            public bool Add { get; set; }
            public bool Delete { get; set; }
            public bool Edit { get; set; }
            public bool Preview { get; set; }
            public bool Publish { get; set; }
            public bool Save { get; set; }
        }

        public class PostPermissions
        {
            public bool Add { get; set; }
            public bool Delete { get; set; }
            public bool Edit { get; set; }
            public bool Preview { get; set; }
            public bool Publish { get; set; }
            public bool Save { get; set; }
        }

        public class SitePermissions
        {
            public bool Add { get; set; }
            public bool Delete { get; set; }
            public bool Edit { get; set; }
            public bool Save { get; set; }
        }

        public AliasPermissions Aliases { get; } = new AliasPermissions();
        public CommentPermissions Comments { get; } = new CommentPermissions();
        public MediaPermissions Media { get; } = new MediaPermissions();
        public PagePermissions Pages { get; } = new PagePermissions();
        public PostPermissions Posts { get; } = new PostPermissions();
        public SitePermissions Sites { get; } = new SitePermissions();
    }
}