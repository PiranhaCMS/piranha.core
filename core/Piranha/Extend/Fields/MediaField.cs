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

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Media", Shorthand = "Media")]
    public class MediaField : MediaFieldBase<MediaField>
    {
        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="str">The string value</param>
        public static implicit operator MediaField(Guid guid)
        {
            return new MediaField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator MediaField(Data.Media media)
        {
            return new MediaField { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="image">The media field</param>
        public static implicit operator string(MediaField image)
        {
            if (image.Media != null)
            {
                return image.Media.PublicUrl;
            }
            return "";
        }
    }
}