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
    /// Image block.
    /// </summary>
    [BlockType(Name = "Image", Category = "Media", Icon = "fas fa-image", Component = "image-block")]
    public class ImageBlock : Block
    {
        /// <summary>
        /// Gets/sets the image body.
        /// </summary>
        public ImageField Body { get; set; }

        public override string GetTitle()
        {
            if (Body != null && Body.Media != null)
            {
                return Body.Media.Filename;
            }

            return "No image selected";
        }
    }
}
