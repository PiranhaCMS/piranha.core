/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System;
using Piranha.Data;

namespace Piranha.Extend.Fields
{
    [Field(Name = "Video", Shorthand = "Video")]
    public class VideoField : MediaFieldBase
    {
        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The guid value</param>
        public static implicit operator VideoField(Guid guid) {
            return new VideoField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator VideoField(Media media) {
            return new VideoField { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="image">The video field</param>
        public static implicit operator string(VideoField image)
        {
            return image.Media != null ? image.Media.PublicUrl : "";
        }
    }
}