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
    public class BlockEditModel
    {
        public bool IsActive { get; set; }
        public Block Model { get; set; }
        public ContentMeta Meta { get; set; }
    }

    public class BlockGroupEditModel : Block
    {
        public IList<BlockEditModel> Items { get; set; } = new List<BlockEditModel>();
        public IList<FieldEditModel> Fields { get; set; } = new List<FieldEditModel>();
    }

    public class RegionEditModel
    {
        public IList<RegionItemEditModel> Items { get; set; } = new List<RegionItemEditModel>();
        public ContentRegionMeta Meta { get; set; }
    }

    public class RegionItemEditModel
    {
        public IList<FieldEditModel> Fields { get; set; } = new List<FieldEditModel>();
    }

    public class FieldEditModel
    {
        public string Type { get; set; }
        public IField Model { get; set; }
        public ContentMeta Meta { get; set; }
    }

    /// <summary>
    /// Content edit model.
    /// </summary>
    public abstract class ContentEditModel
    {
        public Guid Id { get; set; }
        public string TypeId { get; set; }
        public string Title { get; set; }
        public IList<BlockEditModel> Blocks { get; set; } = new List<BlockEditModel>();
        public IList<RegionEditModel> Regions { get; set; } = new List<RegionEditModel>();
    }
}