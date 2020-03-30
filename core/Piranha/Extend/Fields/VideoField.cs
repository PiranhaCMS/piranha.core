/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System;
using Piranha.Models;

namespace Piranha.Extend.Fields
{
    [FieldType(Name = "Video", Shorthand = "Video", Component = "video-field")]
    public class VideoField : MediaFieldBase<VideoField>
    {
        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The guid value</param>
        public static implicit operator VideoField(Guid guid)
        {
            return new VideoField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator VideoField(Media media)
        {
            return new VideoField { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="video">The video field</param>
        public static implicit operator string(VideoField video)
        {
            if (video.Media != null)
            {
                return video.Media.PublicUrl;
            }
            return "";
        }
    }
}