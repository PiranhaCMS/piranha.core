/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Piranha.Extend.Fields;

namespace Piranha.Extend.Blocks
{
    /// <summary>
    /// Block for referencing a post.
    /// </summary>
    [BlockType(Name = "Post link", Category = "Content", Icon = "fas fa-link", Component = "post-block")]
    public class PostBlock : Block
    {
        /// <summary>
        /// Gets/sets the post link.
        /// </summary>
        public PostField Body { get; set; }

        /// <summary>
        /// Gets the title of the block when used in a block group.
        /// </summary>
        /// <returns>The title</returns>
        public override string GetTitle()
        {
            if (Body != null && Body.Post != null)
            {
                return Body.Post.Title;
            }
            return "No post selected";
        }
    }
}
