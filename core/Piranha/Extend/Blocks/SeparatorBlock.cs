/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Separator
    /// </summary>
    [BlockType(Name = "Separator", Category = "Content", Icon = "fas fa-divide", Component = "separator-block")]
    public class SeparatorBlock : Block
    {
        public override string GetTitle()
        {
            return "----";
        }
    }
}
