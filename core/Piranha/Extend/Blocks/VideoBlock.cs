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
    /// Video block.
    /// </summary>
    [BlockType(Name = "Video", Category = "Media", Icon = "fas fa-video", Component = "video-block")]
    public class VideoBlock : Block
    {
        /// <summary>
        /// Gets/sets the video body.
        /// </summary>
        public VideoField Body { get; set; }

        public override string GetTitle()
        {
            if (Body != null && Body.Media != null)
            {
                return Body.Media.Filename;
            }

            return "No video selected";
        }
    }
}
