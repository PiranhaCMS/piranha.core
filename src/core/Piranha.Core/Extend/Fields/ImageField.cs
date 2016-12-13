/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;

namespace Piranha.Extend.Fields
{
    [Field(Name = "Image", Shorthand = "Image")]
    public class ImageField : SimpleField<Guid?>
    {
        public Models.MediaItem Image { get; set; }

        /// <summary>
        /// Converts the given Guid to an image field.
        /// </summary>
        /// <param name="id">The image id</param>
        public static implicit operator ImageField(Guid? id) {
            return new ImageField() { Value = id };
        }

        /// <summary>
        /// Initializes the field for client use.
        /// </summary>
        /// <param name="api">The current api</param>
        public override void Init(IApi api) {
            if (Value.HasValue)
                Image = api.Media.GetById(Value.Value);
        }
    }
}
