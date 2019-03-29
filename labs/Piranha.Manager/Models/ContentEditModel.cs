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
using System.Collections.Generic;
using Piranha.Extend;

namespace Piranha.Manager.Models
{
    /// <summary>
    /// Content edit model.
    /// </summary>
    public abstract class ContentEditModel
    {
        public class BlockItem
        {
            public string Uid { get; set; } = "block-" + Math.Abs(Guid.NewGuid().GetHashCode()).ToString();
            public string Name { get; set; }
            public string Icon { get; set; }
            public string Component { get; set; }
            public bool IsActive { get; set; }
            public string Title { get; set; }
            public Block Model { get; set; }
        }

        public class BlockGroupItem : Block
        {
            public IList<BlockItem> Items { get; set; } = new List<BlockItem>();
        }

        public Guid Id { get; set; }
        public string TypeId { get; set; }
        public string Title { get; set; }
        public IList<BlockItem> Blocks { get; set; } = new List<BlockItem>();
    }
}