/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend;

namespace Piranha.Manager.Models.Content
{
    public class BlockEditModel
    {
        public bool IsActive { get; set; }
        public Block Model { get; set; }
        public ContentMeta Meta { get; set; }
    }
}