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
    [FieldType(Name = "Image", Shorthand = "Image", Component = "image-field")]
    public class ImageField : MediaFieldBase<ImageField>
    {
        /// <summary>
        /// Implicit operator for converting a Guid id to a field.
        /// </summary>
        /// <param name="guid">The guid value</param>
        public static implicit operator ImageField(Guid guid)
        {
            return new ImageField { Id = guid };
        }

        /// <summary>
        /// Implicit operator for converting a media object to a field.
        /// </summary>
        /// <param name="media">The media object</param>
        public static implicit operator ImageField(Media media)
        {
            return new ImageField { Id = media.Id };
        }

        /// <summary>
        /// Impicit operator for converting the field to an url string.
        /// </summary>
        /// <param name="image">The image field</param>
        public static implicit operator string(ImageField image)
        {
            if (image.Media != null)
            {
                return image.Media.PublicUrl;
            }
            return "";
        }

        /// <summary>
        /// Gets the url for a resized version of the image.
        /// </summary>
        /// <param name="api">The api</param>
        /// <param name="width">The requested width</param>
        /// <param name="height">The optional height</param>
        /// <returns>The image url</returns>
        public string Resize(IApi api, int width, int? height = null)
        {
            if (Id.HasValue)
            {
                return api.Media.EnsureVersion(Id.Value, width, height);
            }
            return null;
        }
    }
}