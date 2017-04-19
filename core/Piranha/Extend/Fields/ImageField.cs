/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

namespace Piranha.Extend.Fields
{
    [Field(Name = "Image", Shorthand = "Image")]
    public class ImageField : MediaFieldBase
    {
        /// <summary>
        /// Implicit operator for converting a string id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator ImageField(string str) {
            return new ImageField() { Id = str };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator ImageField(Data.Media media) {
            return new ImageField() { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="image">The image field</param>
        public static implicit operator string(ImageField image) {
            if (image.Media != null)
                return image.Media.PublicUrl;
            return "";
        }
    }
}