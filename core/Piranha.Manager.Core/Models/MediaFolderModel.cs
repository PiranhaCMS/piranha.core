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
    public class MediaFolderModel
    {
        public Guid? ParentId { get; set; }
        public string Name { get; set; }
    }
}