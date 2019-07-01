/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Models;

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Single column quote block.
    /// </summary>
    [BlockGroupType(Name = "Columns", Category = "Content", Icon = "fas fa-columns", Display = BlockDisplayMode.Horizontal)]
    public class ColumnBlock : BlockGroup
    {
    }
}
