/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Collections.Generic;
using Piranha.Extend;

namespace Piranha.Manager.Models.Content
{
    public class BlockGroupEditModel : Block
    {
        public IList<BlockEditModel> Items { get; set; } = new List<BlockEditModel>();
        public IList<FieldEditModel> Fields { get; set; } = new List<FieldEditModel>();
    }
}